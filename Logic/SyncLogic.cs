using McHelper.Domain.Models;
using McHelper.Extensions;

namespace McHelper.Logic;

/// <summary> What needs to happen to a single file to make the server match local. </summary>
public enum DeltaKind { Add, Change, Remove }

/// <summary> One file that differs between local and the server. </summary>
public sealed record FileDelta(string RelativePath, DeltaKind Kind);

/// <summary> The differences for one sync target (mods, config, …). </summary>
public sealed record TargetDiff(string Label, IReadOnlyList<FileDelta> Deltas, int RemoteCount)
{
	public int Total => Deltas.Count;
	public int Adds => Deltas.Count(d => d.Kind == DeltaKind.Add);
	public int Changes => Deltas.Count(d => d.Kind == DeltaKind.Change);
	public int Removes => Deltas.Count(d => d.Kind == DeltaKind.Remove);
}

/// <summary> Live progress while syncing: the file about to be transferred and how far along we are. </summary>
public sealed record SyncProgress(int Done, int Total, string Label, string File, DeltaKind Kind)
{
	public int Percent => Total == 0 ? 100 : (int)(100.0 * Done / Total);
}

/// <summary> The Forge version installed locally vs on the server. </summary>
public sealed record ForgeCheck(string? Local, string? Remote)
{
	/// <summary> Both versions were detected, so a comparison is meaningful. </summary>
	public bool Known => Local is not null && Remote is not null;

	/// <summary> Local and server run the same Forge. </summary>
	public bool Match => Known && Local == Remote;
}

/// <summary>
/// Outcome of a diff/sync attempt. <see cref="Error"/> is null on success; when set, the server
/// could not be reached (so <see cref="Targets"/> is empty and the counts must not be trusted).
/// </summary>
public sealed record SyncResult(IReadOnlyList<TargetDiff> Targets, ForgeCheck? Forge, string? Error)
{
	public bool Ok => Error is null;
	public int Total => Targets.Sum(t => t.Total);
}

/// <summary>
/// Mirrors the local Minecraft folders to the server over whatever <see cref="ITransport"/> it is given.
/// A file "differs" when it is missing on one side or its size differs — so edited files re-upload.
/// </summary>
public sealed class SyncLogic(ITransport transport, ServerConfig config, Paths paths)
{
	// Mirror = also delete remote files missing locally. Off for config/serverconfig because the
	// server generates files there (loot tables, .bak, per-mod server configs) that aren't local —
	// those folders are upload/update-only so a sync never deletes server-generated content.
	private sealed record Target(string Label, string From, string To, IReadOnlySet<string>? Only, bool Mirror);

	//the four things we sync, each mapping a local folder to a folder under the server's BasePath.
	//modFiles is the exact set of mod jars to push (all present jars minus client-only mods, decided by the caller)
	private IEnumerable<Target> Targets(IReadOnlySet<string> modFiles)
	{
		yield return new("Mods", Local("mods"), Remote("mods"), modFiles, Mirror: true);
		yield return new("Config", Local("config"), Remote("config"), null, Mirror: false);
		yield return new("Datapacks", Local("saves/0/datapacks"), Remote("world/datapacks"), null, Mirror: true);
		yield return new("Serverconfig", Local("saves/0/serverconfig"), Remote("world/serverconfig"), null, Mirror: false);
	}

	private string Local(string sub) => Path.Combine(paths.McRoot, sub);
	private string Remote(string sub) => $"{config.BasePath.TrimEnd('/')}/{sub}";

	//where both client and server keep the installed Forge, named by version e.g. "1.20.1-47.4.10"
	private const string ForgeLibs = "libraries/net/minecraftforge/forge";

	/// <summary> Detect the Forge version locally and on the server and compare them. </summary>
	public ForgeCheck CheckForge()
	{
		var local = PickForge(LocalForgeVersions());
		var remote = PickForge(transport.ListDirs(Remote(ForgeLibs)));
		var check = new ForgeCheck(local, remote);

		if (check.Known && !check.Match)
			Logger.LogLine($"\n⚠ Forge mismatch — local {local}, server {remote}", ConsoleColor.Red);
		else if (check.Match)
			Logger.LogLine($"Forge {local} matches server", ConsoleColor.DarkGray);
		else if (local is not null && remote is null)
			Logger.LogLine($"Forge {local} locally — could not find Forge on the server", ConsoleColor.DarkYellow);

		return check;
	}

	private IEnumerable<string> LocalForgeVersions()
	{
		var dir = Local(ForgeLibs);
		return Directory.Exists(dir)
			? Directory.GetDirectories(dir).Select(d => Path.GetFileName(d)!)
			: [];
	}

	//newest version wins if more than one is installed (ordinal works for the X.Y.Z-style names)
	private static string? PickForge(IEnumerable<string> versions) =>
		versions.Where(v => !string.IsNullOrEmpty(v))
			.OrderByDescending(v => v, StringComparer.Ordinal)
			.FirstOrDefault();

	/// <summary> Inspect every target and report what differs, without changing anything. </summary>
	public IReadOnlyList<TargetDiff> Diff(IReadOnlySet<string> modFiles)
	{
		var result = new List<TargetDiff>();
		foreach (var target in Targets(modFiles))
		{
			var diff = DiffTarget(target);
			Logger.LogLine($"{target.Label}: {Describe(diff)}",
				diff.Total == 0 ? ConsoleColor.DarkGray : ConsoleColor.Magenta);
			result.Add(diff);
		}
		return result;
	}

	/// <summary> Apply every target's diff to the server, reporting progress per file, then report what was synced. </summary>
	public IReadOnlyList<TargetDiff> SyncAll(IReadOnlySet<string> modFiles, IProgress<SyncProgress>? progress = null)
	{
		//diff everything up front so we know the grand total before the first upload
		var plan = Targets(modFiles).Select(t => (target: t, diff: DiffTarget(t))).ToArray();
		var total = plan.Sum(p => p.diff.Total);

		if (total == 0)
		{
			Logger.LogLine("nothing to sync — already up to date", ConsoleColor.Green);
			progress?.Report(new(0, 0, "", "", DeltaKind.Add));
			return plan.Select(p => p.diff).ToList();
		}

		var done = 0;
		foreach (var (target, diff) in plan)
		{
			if (diff.Total == 0)
				continue;

			Logger.LogLine($"\n{target.Label}: {Describe(diff)}", ConsoleColor.Magenta);
			foreach (var d in diff.Deltas)
			{
				progress?.Report(new(done, total, target.Label, d.RelativePath, d.Kind));
				ApplyOne(target, d);
				done++;
			}
		}

		progress?.Report(new(done, total, "", "", DeltaKind.Add));
		Logger.LogLine("\nsync complete", ConsoleColor.Green);
		return plan.Select(p => p.diff).ToList();
	}

	private TargetDiff DiffTarget(Target target)
	{
		var local = LocalFiles(target.From, target.Only);
		var remote = transport.List(target.To);

		var deltas = new List<FileDelta>();
		foreach (var (rel, size) in local)
		{
			if (!remote.TryGetValue(rel, out var remoteSize))
				deltas.Add(new(rel, DeltaKind.Add));
			else if (remoteSize != size)
				deltas.Add(new(rel, DeltaKind.Change));
		}

		//delete remote extras only for mirror targets, and only when we actually manage the folder
		//locally (an empty local source means "not managed here", so leave the server's files alone)
		if (target.Mirror && local.Count > 0)
			foreach (var rel in remote.Keys)
				if (!local.ContainsKey(rel))
					deltas.Add(new(rel, DeltaKind.Remove));

		deltas.Sort((a, b) => string.CompareOrdinal(a.RelativePath, b.RelativePath));
		return new(target.Label, deltas, remote.Count);
	}

	private void ApplyOne(Target target, FileDelta d)
	{
		var remote = $"{target.To}/{d.RelativePath}";
		if (d.Kind == DeltaKind.Remove)
		{
			transport.Delete(remote);
			Logger.LogLine($"  - {d.RelativePath}", ConsoleColor.DarkRed);
		}
		else
		{
			transport.Upload(Path.Combine(target.From, d.RelativePath), remote);
			var tag = d.Kind == DeltaKind.Add ? "+" : "~";
			Logger.LogLine($"  {tag} {d.RelativePath}", ConsoleColor.DarkGreen);
		}
	}

	//local files relative to root (forward slashes) with size, optionally limited to a filename set
	private static Dictionary<string, long> LocalFiles(string root, IReadOnlySet<string>? only)
	{
		var result = new Dictionary<string, long>(StringComparer.Ordinal);
		if (!Directory.Exists(root))
			return result;

		foreach (var full in Directory.GetFiles(root, "*.*", SearchOption.AllDirectories))
		{
			var rel = Path.GetRelativePath(root, full).Replace('\\', '/');
			if (only != null && !only.Contains(rel))
				continue;
			result[rel] = new FileInfo(full).Length;
		}
		return result;
	}

	private static string Describe(TargetDiff diff)
	{
		if (diff.Total == 0)
			return "no differences";
		var add = diff.Deltas.Count(d => d.Kind == DeltaKind.Add);
		var chg = diff.Deltas.Count(d => d.Kind == DeltaKind.Change);
		var rem = diff.Deltas.Count(d => d.Kind == DeltaKind.Remove);
		var parts = new List<string>();
		if (add > 0) parts.Add($"{add} new");
		if (chg > 0) parts.Add($"{chg} changed");
		if (rem > 0) parts.Add($"{rem} to remove");
		return string.Join(", ", parts);
	}
}

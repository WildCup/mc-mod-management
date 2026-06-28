using McHelper.Domain.Models;
using McHelper.Extensions;

namespace McHelper.Logic;

/// <summary>
/// In-memory db state and the actions the UI triggers.
/// Logical mods are matched to live jar files in the mods folder by their known filenames;
/// "installed" is observed from that folder, never stored.
/// </summary>
public class ModService(ModsRepo repo, ServerConfig server, Paths paths)
{
	public List<Mod> Mods { get; private set; } = [];

	/// <summary> Present jar filenames that no logical mod claims yet — need the user to assign them. </summary>
	public List<string> Unassigned { get; private set; } = [];

	private HashSet<string> _installed = new(StringComparer.InvariantCultureIgnoreCase);

	/// <summary> Load db, scan folder, then match files to logical mods. </summary>
	public void Refresh()
	{
		Mods = repo.Load();
		_installed = repo.ScanInstalled().ToHashSet(StringComparer.InvariantCultureIgnoreCase);
		Recompute();
		Logger.LogLine(
			$"DB: {Mods.Count} mods, {_installed.Count} installed, {Unassigned.Count} unassigned",
			ConsoleColor.Magenta);
	}

	/// <summary> Re-match logical mods to the last scan. Call after any edit that changes filenames. </summary>
	public void Recompute()
	{
		var claimed = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var mod in Mods)
		{
			mod.Installed = !string.IsNullOrEmpty(mod.FileName) && _installed.Contains(mod.FileName);
			//restart since the mod was removed
			if (!mod.Installed)
			{
				mod.Works = false;
				mod.Configured = false;
			}
			if (!string.IsNullOrEmpty(mod.FileName))
				claimed.Add(mod.FileName);
			//files referenced as a dependency are accounted for too
			foreach (var dep in mod.Dependencies)
				claimed.Add(dep);
		}

		Unassigned = _installed
			.Where(f => !claimed.Contains(f))
			.OrderBy(f => f, StringComparer.InvariantCultureIgnoreCase)
			.ToList();
	}

	/// <summary> Whether a given jar filename is currently present in the mods folder. </summary>
	public bool IsPresent(string filename) => _installed.Contains(filename);

	public Mod? Find(string id) => Mods.FirstOrDefault(m => m.IsSameMod(id));

	/// <summary> Claim a present filename for a logical mod, overriding its file; creates the mod if the id is new. </summary>
	public void Assign(string filename, string modId)
	{
		modId = modId.Trim();
		if (string.IsNullOrEmpty(modId))
			return;

		var mod = Find(modId);
		if (mod is null)
		{
			mod = new Mod { Id = modId };
			Mods.Add(mod);
		}

		mod.FileName = filename;

		Recompute();
		Logger.LogLine($"Assigned {filename} -> {mod.Id}", ConsoleColor.Green);
	}

	/// <summary> Dependency filenames of an installed mod that are not present in the folder. </summary>
	public IEnumerable<string> MissingDependencies(Mod mod)
	{
		if (!mod.Installed)
			yield break;

		foreach (var dep in mod.Dependencies)
			if (!_installed.Contains(dep))
				yield return dep;
	}

	/// <summary>
	/// A present dependency-category mod that nothing installed depends on — likely left behind
	/// by accident after its dependents were removed.
	/// </summary>
	public bool IsFloating(Mod mod)
	{
		if (!mod.Installed || mod.Category != Category.Dependency)
			return false;

		return !Mods.Any(m => m.Installed
			&& m.Dependencies.Contains(mod.FileName, StringComparer.InvariantCultureIgnoreCase));
	}

	public void Save() => repo.Save(Mods);

	public void Delete(Mod mod)
	{
		_ = Mods.Remove(mod);
		Recompute();
		Logger.LogLine($"Removed {mod.Id} from db", ConsoleColor.Red);
	}

	/// <summary>
	/// Every jar present in the local mods folder except those belonging to client-only mods.
	/// The server needs all the libraries/dependencies too, not just the mods that have a db entry —
	/// so we push the whole folder and only hold back what's explicitly marked <see cref="Category.Client"/>.
	/// </summary>
	private IReadOnlySet<string> ServerModFiles()
	{
		var clientFiles = Mods
			.Where(m => m.Category == Category.Client && !string.IsNullOrEmpty(m.FileName))
			.Select(m => m.FileName)
			.ToHashSet(StringComparer.InvariantCultureIgnoreCase);

		return _installed
			.Where(f => !clientFiles.Contains(f))
			.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
	}

	/// <summary> Compare every local folder (and the Forge version) against the server without changing anything. </summary>
	public SyncResult Diff() => WithSync(sync => (sync.Diff(ServerModFiles()), sync.CheckForge()));

	/// <summary> Push every local folder to the server in one go (reporting progress), then report what was synced. </summary>
	public SyncResult SyncAll(IProgress<SyncProgress>? progress = null) =>
		WithSync(sync => (sync.SyncAll(ServerModFiles(), progress), sync.CheckForge()));

	private SyncResult WithSync(Func<SyncLogic, (IReadOnlyList<TargetDiff> Targets, ForgeCheck Forge)> action)
	{
		try
		{
			using var transport = TransportFactory.Connect(server);
			var sync = new SyncLogic(transport, server, paths);
			var (targets, forge) = action(sync);
			return new(targets, forge, null);
		}
		catch (Exception ex)
		{
			Logger.LogLine($"sync failed: {ex.Message}", ConsoleColor.Red);
			return new([], null, ex.Message);
		}
	}
}

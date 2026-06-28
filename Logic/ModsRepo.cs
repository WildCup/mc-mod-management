using System.Text.Json;
using McHelper.Domain.Models;
using McHelper.Extensions;

namespace McHelper.Logic;

/// <summary>
/// Loads/saves the mods from database (json files) and .minecraft folder.
/// </summary>
public class ModsRepo(Paths paths)
{
	private readonly JsonSerializerOptions _jsonOptions =
		new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

	/// <summary> Load all known mods from database, migrating the legacy filename-keyed format. </summary>
	public List<Mod> Load()
	{
		if (!File.Exists(paths.KnownMods))
			return [];

		var json = File.ReadAllText(paths.KnownMods);
		using var doc = JsonDocument.Parse(json);
		if (doc.RootElement.GetArrayLength() == 0)
			return [];

		var stored = JsonSerializer.Deserialize<List<Mod>>(json, _jsonOptions)
			?? throw new YouIdiotException("Database could not be read!");
		return stored;
	}

	/// <summary> Save all known mods to database </summary>
	public void Save(IEnumerable<Mod> mods)
	{
		var ordered = mods.OrderBy(m => m.Id, StringComparer.InvariantCultureIgnoreCase).ToList();

		//backup current db before overwriting
		if (File.Exists(paths.KnownMods))
			File.Copy(paths.KnownMods, paths.Backup, overwrite: true);

		File.WriteAllText(paths.KnownMods, JsonSerializer.Serialize(ordered, _jsonOptions));
		Logger.LogLine($"Database saved ({ordered.Count} mods)");
	}

	/// <summary> File names currently present in the .minecraft/mods folder. </summary>
	public string[] ScanInstalled()
	{
		return Directory.GetFiles(paths.McLocalMods)
			.Select(x => Path.GetFileName(x) ?? "")
			.Where(x => x.Length > 0)
			.ToArray();
	}
}

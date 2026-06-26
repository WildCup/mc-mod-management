using System.Text.Json;
using McHelper.Extensions;

namespace McHelper.Logic;

/// <summary>
/// Responsible for saving/loading mods from json files.
/// Acts as 'database'.
/// </summary>
public class FileLogic
{
	private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

	public IReadOnlyCollection<Mod> ModsInput { get; set; }
	public IReadOnlyCollection<Mod> Mods { get; set; }
	public IReadOnlyCollection<Mod> ModsKnown { get; set; }

	public FileLogic()
	{
		//load
		var modsJson = File.ReadAllText(Options.ModsPath);
		var knownJson = File.ReadAllText(Options.KnownPath);

		Mods = JsonSerializer.Deserialize<List<Mod>>(modsJson) ?? throw new YouIdiotException("Saved mods not found!!");
		ModsKnown = JsonSerializer.Deserialize<List<Mod>>(knownJson) ?? throw new YouIdiotException("Saved known mods not found!!");
		ModsInput = GetInputMods().ToList();

		if (Mods.Count == 0 || ModsKnown.Count == 0 || ModsInput.Count == 0)
			throw new YouIdiotException("Something went wrong with reading mods");

		File.WriteAllText(Options.BackupPath, modsJson);
		Logger.LogLine($"Mods: {Mods.Count} ModsKnown: {ModsKnown.Count} ModsInput: {ModsInput.Count}");
	}

	public void Save(IEnumerable<Mod> mods, IEnumerable<Mod> modsKnown)
	{
		mods = mods.OrderBy(file => file.Name);
		modsKnown = modsKnown.OrderBy(file => file.Name);

		var modsJson = JsonSerializer.Serialize(mods, _jsonOptions);
		var modsKnownJson = JsonSerializer.Serialize(modsKnown, _jsonOptions);

		File.WriteAllText(Options.ModsPath, modsJson);
		File.WriteAllText(Options.KnownPath, modsKnownJson);

		Logger.LogLine("Mods saved in database successfully");
	}

	//read all mods in folder .minecraft/mods
	private static IEnumerable<Mod> GetInputMods()
	{
		var fileNames = Directory.GetFiles(Options.InputPath).Select(x => Path.GetFileName(x) ?? "").Except(Options.Ignore).ToArray();
		return fileNames.Select(x => new Mod() { Name = x });
	}
}


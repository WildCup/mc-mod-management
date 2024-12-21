using System.Text.Json;
using System.Xml;
using McHelper.Domain.Models.Enums;

namespace McHelper;

public class FileLogic
{
	private readonly string[] _names;
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
		_names = Directory.GetFiles(Options.InputPath).Select(x => Path.GetFileName(x) ?? "").Except(Options.Ignore).ToArray();

		if (Mods.Count == 0 || ModsKnown.Count == 0 || _names.Length == 0 || _names.Contains(""))
			throw new YouIdiotException("Something went wrong with reading mods");

		File.WriteAllText(Options.BackupPath, modsJson);


		ModsInput = GetInputMods().ToList();
	}

	public void Save(IEnumerable<Mod> mods, IEnumerable<Mod> modsKnown)
	{
		mods = mods.OrderBy(file => file.Name);
		modsKnown = modsKnown.OrderBy(file => file.Name);

		var modsJson = JsonSerializer.Serialize(mods, _jsonOptions);
		var modsKnownJson = JsonSerializer.Serialize(modsKnown, _jsonOptions);
		var tmpJson = JsonSerializer.Serialize(mods.Where(x => x.Category == Category.Unknown || !x.WorksConfirmed), _jsonOptions);

		File.WriteAllText(Options.ModsPath, modsJson);
		File.WriteAllText(Options.KnownPath, modsKnownJson);
		File.WriteAllText(@"./Db/tmp.json", tmpJson);

		Console.WriteLine("File names saved to JSON successfully");
	}

	//read all mods in folder .minecraft/mods
	private IEnumerable<Mod> GetInputMods()
	{
		return _names.Select(x => new Mod()
		{
			Name = x
		});
	}
}


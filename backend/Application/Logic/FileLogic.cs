namespace McHelper.Application.Logic;

using McHelper.Domain.Exceptions;
using McHelper.Domain.Extensions;
using McHelper.Domain.Models;

public class FileLogic
{
	private static readonly string ModsPath = ModExtensions.GetPath() + @"\installed.json";
	private static readonly string KnownPath = ModExtensions.GetPath() + @"\known.json";
	private static readonly string BackupPath = ModExtensions.GetPath() + @"\backup.json";
	private static readonly string[] Ignore = ["tl_skin_cape_forge", "OptiFine-OptiFine", "DynamicSurroundings-1.16.5-4.0.5.0.jar", "Hwyla-forge-1.10.11-B78_1.16.2.jar", "Sledgehammer-1.16.5-2.0.1.jar"];
	private readonly string[] _names;

	public IReadOnlyCollection<McMod> ModsInput { get; set; }
	public IReadOnlyCollection<McMod> Mods { get; set; }
	public IReadOnlyCollection<McMod> ModsKnown { get; set; }

	public FileLogic(string input)
	{
		//init
		var modsData = File.ReadAllText(ModsPath);
		var knownData = File.ReadAllText(KnownPath);

		Mods = JsonConvert.DeserializeObject<List<McMod>>(modsData) ?? [];
		ModsKnown = JsonConvert.DeserializeObject<List<McMod>>(knownData) ?? [];
		_names = Directory.GetFiles(input).Select(n => Path.GetFileName(n) ?? "").Except(Ignore).ToArray();

		if (Mods.Count == 0 || ModsKnown.Count == 0 || _names.Length == 0 || _names.Contains(""))
			throw new IncorrectConfigException(Mods.Count, ModsKnown.Count, _names);

		File.WriteAllText(BackupPath, modsData);

		//check for duplicates
		var duplicates = _names.Where(n => _names.Where(n2 => n2.SameName(n)).Count() > 1);
		if (duplicates.Any())
			throw new DuplicatesException(duplicates);

		ModsInput = new ReadOnlyCollection<McMod>(GetBaseMods(input));
	}

	public static void Save(IEnumerable<McMod> mods, IEnumerable<McMod> modsKnown)
	{
		var modsJson = JsonConvert.SerializeObject(mods.OrderBy(file => file.Name).ToArray(), Formatting.Indented);
		var modsKnownJson = JsonConvert.SerializeObject(modsKnown.OrderBy(file => file.Name).ToArray(), Formatting.Indented);

		File.WriteAllText(ModsPath, modsJson);
		File.WriteAllText(KnownPath, modsKnownJson);

		Console.WriteLine("File names saved to JSON successfully");
	}

	private List<McMod> GetBaseMods(string input) //read metadata from all mods in folder .minecraft/mods
	{
		ModExtensions.Log($"Synchronizing dependencies", ConsoleColor.DarkMagenta);

		var modsInput = new List<McMod>();
		foreach (var name in _names)
		{
			var zip = ZipFile.OpenRead($"{input}\\{name}") ?? throw new PathNotFoundException($"file {name} could not be opened");
			var model = ZipLogic.GetMetaToml(zip, name);
			if (model != null)
			{
				var modId = ZipLogic.GetId(model, name);
				var dependencies = ZipLogic.GetDependencies(model).ToList();
				modsInput.Add(new() { Id = modId, Name = name, Dependencies = dependencies });

				continue;
			}
			var json = ZipLogic.GetMetaJson(zip, name);
			if (json != null)
			{
				var modId = ZipLogic.GetId(json, name);
				var dependencies = ZipLogic.GetDependencies(json, name).ToList();
				modsInput.Add(new() { Id = modId, Name = name, Dependencies = dependencies });

				continue;
			}

			ModExtensions.Log($"{name} skipped", ConsoleColor.DarkGray);
		}

		//update dependencies to be full names rather then ids
		var map = new Dictionary<string, string>(); //id -> fullName
		foreach (var mod in modsInput)
			map.Add(mod.Id, mod.Name);

		foreach (var mod in modsInput)
		{
			foreach (var dependencyId in mod.Dependencies.ToArray())
			{
				if (!map.ContainsKey(dependencyId))
				{
					ModExtensions.Log($"Dependency {dependencyId} required in {mod.Name} but not installed", ConsoleColor.DarkRed);
					continue;
				}

				mod.Dependencies.Remove(dependencyId);
				mod.Dependencies.Add(map[dependencyId]);
			}
		}

		return modsInput;
	}
}


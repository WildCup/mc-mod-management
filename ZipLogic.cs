namespace McHelper;

public class ZipLogic
{
	private static readonly string[] Ignore = new string[] { "forge", "minecraft", "fabricloader", "java", "fabric", "fabric-api", "fabric-lifecycle-events-v1", "fabric-command-api-v2", "fabric-api-base", "fabric-api" };

	public static TomlTable? GetMetaToml(ZipArchive zip, string name) //read metadata from all mods in folder .minecraft/mods
	{
		var meta = zip.Entries.FirstOrDefault(e => e.FullName.Equals("META-INF/mods.toml", StringComparison.OrdinalIgnoreCase));
		if (meta == null)
			return null;

		var text = new string(new StreamReader(meta.Open(), Encoding.UTF8).ReadToEnd().ToArray()) ?? throw new MetaDataException(name);

		var model = Toml.ToModel(text);
		return model;
	}

	public static MetaJson? GetMetaJson(ZipArchive zip, string name)
	{
		var meta = zip.Entries.FirstOrDefault(e => e.FullName.Equals("fabric.mod.json", StringComparison.OrdinalIgnoreCase));
		if (meta == null)
			return null;

		var text = new string(new StreamReader(meta.Open(), Encoding.UTF8).ReadToEnd()) ?? throw new MetaDataException(name);

		var json = JsonConvert.DeserializeObject<MetaJson>(text) ?? throw new MetaDataException(meta.FullName);

		return json;
	}

	public static IEnumerable<string> GetDependencies(TomlTable model)
	{
		var found = new List<string>();
		if (!model.ContainsKey("dependencies"))
			return found;

		var dependencies = (TomlTable)model["dependencies"];
		var values = dependencies.Values;
		foreach (var value in values)
		{
			foreach (var table in ((TomlTableArray)value).AsEnumerable())
			{
				if ((bool)table["mandatory"])
					found.Add((string)table["modId"]);
			}
		}
		return found.Except(Ignore).ToList();
	}
	public static string GetId(TomlTable model, string name)
	{
		var idSection = model["mods"];
		var found = new List<string>();

		if (idSection is TomlArray array)
		{
			foreach (var element in array)
			{
				if (element == null)
					continue;
				var table = (TomlTable)element;
				found.Add((string)table["modId"]);
			}
		}
		else
		{
			var value = (TomlTableArray)model["mods"];

			foreach (var table in value.AsEnumerable())
				found.Add((string)table["modId"]);
		}

		if (found.Count < 1)
			throw new MetaDataException(model, name);
		return found.First();
	}
	public static IEnumerable<string> GetDependencies(MetaJson json, string name)
	{
		if (json.Depends == null)
			throw new MetaDataException(json, name);
		var dependencies = json.Depends.Keys.ToList();
		return dependencies.Except(Ignore.Concat(new[] { name })).ToList();
	}
	public static string GetId(MetaJson json, string name)
	{
		if (json.Id == null)
			throw new MetaDataException(json, name);
		return json.Id;
	}
}


public class MetaJson
{
	public string? Id { get; set; }
	public Dictionary<string, string>? Depends { get; set; }
}

namespace McHelper.Models;

public class McMod
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public Priority Priority { get; set; }
	public bool WorksConfirmed { get; set; }
	[JsonConverter(typeof(StringEnumConverter))]
	public Category Category { get; set; }
	public string Description { get; set; } = string.Empty;
	public List<string> Dependencies { get; set; } = new List<string>();
	[JsonIgnore]
	public TomlTable? MetaData { get; set; }

	public bool HasExactName(string name)
	{
		return Name.Equals(name, StringComparison.OrdinalIgnoreCase);
	}
	public bool HasName(string name)
	{
		return Name.SameName(name);
	}
	public bool IsSameMod(McMod mod)
	{
		return HasName(mod.Name) || (!string.IsNullOrEmpty(Id) && Id.Equals(mod.Id, StringComparison.OrdinalIgnoreCase));
	}
}

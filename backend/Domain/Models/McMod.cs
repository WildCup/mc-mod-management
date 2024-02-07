namespace McHelper.Domain.Models;

using McHelper.Domain.Extensions;

public class McMod
{
	public required int Id { get; set; }
	public required string Name { get; set; }
	public required string ForgeId { get; set; }
	public string Description { get; set; } = string.Empty;
	public Priority Priority { get; set; }
	public Category Category { get; set; }
	public bool WorksConfirmed { get; set; }
	public List<int> Dependencies { get; set; } = [];
	public TomlTable? MetaData { get; set; }

	public bool HasExactName(string name)
	{
		return Name.Equals(name, StringComparison.OrdinalIgnoreCase);
	}

	public bool IsSameMod(McMod mod)
	{
		return Name.SameName(mod.Name) || ForgeId.Equals(mod.ForgeId, StringComparison.OrdinalIgnoreCase);
	}
}


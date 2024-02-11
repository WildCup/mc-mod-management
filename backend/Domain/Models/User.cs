namespace McHelper.Domain.Models;

public class User : EntityBase
{
	public required string Name { get; set; }
	public List<Mod> Mods { get; set; } = [];
}

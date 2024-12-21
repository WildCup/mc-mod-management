using McHelper.Domain.Models.Enums;

public class Mod
{
	public required string Name { get; set; }
	public Priority Priority { get; set; } = Priority.Unknown;
	public bool WorksConfirmed { get; set; }
	public Category Category { get; set; } = Category.Unknown;
	public string Description { get; set; } = string.Empty;
	public List<string> Dependencies { get; set; } = [];

	public bool IsSameMod(Mod other)
	{
		return Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
	}
}


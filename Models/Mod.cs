using McHelper.Domain.Models;

/// <summary>
/// Each 'Mod' instance represents 1 file in .minecraft/mods folder
/// Name is used as the mod unique id for lookup
/// Rest is manually edited (directly in json file)
/// </summary>
public class Mod
{
	/// <summary> The exact file name (same as in forge) </summary>
	public required string Name { get; set; }

	/// <summary> How important is this mod for this mudpack (can be potentially deleted?) </summary>
	public Priority Priority { get; set; } = Priority.Unknown;

	/// <summary> Whether this mod was tested to work in current mudpack </summary>
	public bool WorksConfirmed { get; set; }

	/// <summary> What this mod adds to the game (Content/Look/Difficulty..) </summary>
	public Category Category { get; set; } = Category.Unknown;

	/// <summary> Short summary of the mod </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary> Exact file name of all dependencies </summary>
	public List<string> Dependencies { get; set; } = [];

	public bool IsSameMod(Mod other)
	{
		return Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
	}
}


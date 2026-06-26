using System.Text.Json.Serialization;

namespace McHelper.Domain.Models;

/// <summary>
/// Each 'Mod' instance represents 1 file in .minecraft/mods folder
/// Name is used as the mod unique id for lookup
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

	/// <summary>
	/// Whether the file currently exists in the .minecraft/mods folder.
	/// Computed live from a folder scan, never persisted to the db.
	/// </summary>
	[JsonIgnore]
	public bool Installed { get; set; }

	public bool IsSameMod(Mod other) => IsSameMod(other.Name);

	public bool IsSameMod(string otherName) =>
		Name.Equals(otherName, StringComparison.InvariantCultureIgnoreCase);
}

using System.Text.Json.Serialization;

namespace McHelper.Domain.Models;

/// <summary>
/// A logical mod, identified by a custom name chosen with intent (ex. "MineColonies").
/// </summary>
public class Mod
{
	/// <summary> The custom identity name. Stable across versions, used for lookup. </summary>
	public required string Id { get; set; }

	/// <summary> How important is this mod for this mudpack (can be potentially deleted?) </summary>
	public Priority Priority { get; set; } = Priority.Unknown;

	/// <summary> What this mod adds to the game (Content/Look/Difficulty..) </summary>
	public Category Category { get; set; } = Category.Unknown;

	/// <summary> Short summary of the mod </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary> The exact jar filename for this mod </summary>
	public string FileName { get; set; } = string.Empty;

	/// <summary> Exact jar filenames this mod depends on </summary>
	public List<string> Dependencies { get; set; } = [];

	/// <summary> Tested and confirmed working </summary>
	public bool Works { get; set; }

	/// <summary> All configs checked and final, won't be modified again </summary>
	public bool Configured { get; set; }

	/// <summary>
	/// Whether this mod's <see cref="FileName"/> is currently present in the mods folder.
	/// Computed live from a folder scan, never persisted.
	/// </summary>
	[JsonIgnore]
	public bool Installed { get; set; }

	public bool IsSameMod(Mod other) => IsSameMod(other.Id);

	public bool IsSameMod(string otherId) =>
		Id.Equals(otherId, StringComparison.InvariantCultureIgnoreCase);
}

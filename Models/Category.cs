using System.Text.Json.Serialization;

namespace McHelper.Domain.Models;

/// <summary>
/// Describes the purpose of the mod 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Category
{
	/// <summary> To be determined (default value) </summary>
	Unknown = 0,

	/// <summary> Adds more content to the game </summary>
	Content = 1,

	/// <summary> Adds beautiful blocks, structures, biomes etc. </summary>
	Look = 2,

	/// <summary> Adds very convenient mechanics </summary>
	Convention = 3,

	/// <summary> Makes the game more difficult </summary>
	Difficulty = 4,

	/// <summary> Makes the game more optimized </summary>
	Helper = 5,

	/// <summary> Dependencies for other mods </summary>
	Dependency = 6,

	/// <summary> Client side mods (not synced to the server) </summary>
	Client = 7,
}

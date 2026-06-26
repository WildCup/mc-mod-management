using System.Text.Json.Serialization;

namespace McHelper.Domain.Models;

/// <summary>
/// Determines how important this mod is.
/// Highest priorities mods should be tested first if they work.
/// Other mods only complement the core mods but might be removed or changed as needed.
/// ex. if server crashes or there is just too much, or they are bad.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Priority
{
	/// <summary> To be determined (default value) </summary>
	Unknown = 0,

	/// <summary> Useless mod to be removed </summary>
	NotNeeded = 1,

	/// <summary> Potentially nice mod but not sure yet if it should stay </summary>
	Maybe = 2,

	/// <summary> Core of the modpack - best, most important mods </summary>
	Necessary = 3,
}

namespace McHelper.Domain.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum Category
{
	Unknown = 0, //to be determined
	Content = 1, //adds more content to the game
	Look = 2, //adds beautiful blocks, structures, biomes etc.
	Convention = 3, //adds very convenient mechanics
	Difficulty = 4, //makes the game more difficult
	Helper = 5, //makes the game more optimized
	Dependency = 6, //dependencies for other mods
	Client = 7, //client side mods
}

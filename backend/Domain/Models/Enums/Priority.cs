namespace McHelper.Domain.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum Priority
{
	Unknown = 0,
	Maybe = 1,
	Cool = 2,
	Necessary = 3,
}

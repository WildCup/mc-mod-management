using System.Text.Json.Serialization;

namespace McHelper.Domain.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Priority
{
	Unknown = 0,
	NotNeeded = 1,
	Maybe = 2,
	Necessary = 3,
}

namespace McHelper.Domain.Exceptions;

using McHelper.Domain.Models;

public class IncorrectConfigException(int modsCount, int knownCount, string[] names) : Exception(GetMsg(modsCount, knownCount, names))
{
	private static string GetMsg(int modsCount, int knownCount, string[] names)
	{
		var msg = "couldn't read data: ";
		if (modsCount == 0)
			msg += $"\n-logic.Mods";
		if (knownCount == 0)
			msg += $"\n-logic.ModsKnown";
		if (names.Length == 0 || names.Contains(""))
			msg += $"\n-logic.Names";
		return msg;
	}
}

public class DuplicatesException(IEnumerable<string> duplicates) : Exception(GetMsg(duplicates))
{
	private static string GetMsg(IEnumerable<string> duplicates)
	{
		var msg = "you have duplicates: ";
		foreach (var duplicate in duplicates)
			msg += $"\n-{duplicate}";
		return msg;
	}
}

public class MetaDataException : Exception
{
	public MetaDataException(string name) : base($"could not open zip {name}") { }

	public MetaDataException(TomlTable model, string name) : base(GetMsg(model, name)) { }
	private static string GetMsg(TomlTable model, string name)
	{
		var msg = $"something is wrong with model of {name}: ";
		return msg + "\n" + JsonConvert.SerializeObject(model, Formatting.Indented);
	}

	public MetaDataException(MetaJson json, string name) : base(GetMsg(json, name)) { }
	private static string GetMsg(MetaJson json, string name)
	{
		var msg = $"something is wrong with json of {name}: ";
		return msg + "\n" + JsonConvert.SerializeObject(json, Formatting.Indented);
	}
}

public class PathNotFoundException(string? msg = null) : Exception(msg ?? GetMsg())
{
	private static string GetMsg()
	{
		return "could not find a path in location: " + Assembly.GetExecutingAssembly().Location;
	}
}

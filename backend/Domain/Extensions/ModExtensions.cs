namespace McHelper.Domain.Extensions;

using McHelper.Domain.Exceptions;

public static class ModExtensions
{
	private static readonly System.Buffers.SearchValues<char> MyChars = System.Buffers.SearchValues.Create("0123456789");

	public static string PureName(this string str)
	{
		if (!"0123456789".ToCharArray().Any(i => str.Contains(i)))
			return str;
		str = str[..str.AsSpan().IndexOfAny(MyChars)];
		if (str.EndsWith("-", StringComparison.OrdinalIgnoreCase) || str.EndsWith("_", StringComparison.OrdinalIgnoreCase))
			return str[..^1];
		return str;
	}

	public static bool SameName(this string str, string str2)
	{
		return str.PureName().Equals(str2.PureName(), StringComparison.OrdinalIgnoreCase);
	}

	public static bool ContainsName(this IEnumerable<string> arr, string name)
	{
		return arr.Any(n => n.SameName(name));
	}

	public static List<string> Logs { get; set; } = [];
	public static void Log(string text, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.WriteLine(text);
		Logs.Add(text);
	}
	public static string GetPath()
	{
		var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new PathNotFoundException();
		if (path.Contains("bin"))
			path = path[..path.IndexOf(@"\bin", StringComparison.OrdinalIgnoreCase)] ?? throw new PathNotFoundException();
		return path;
	}
}

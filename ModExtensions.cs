namespace McHelper;

public static class ModExtensions
{
    public static string PureName(this string str)
    {
        if (!"0123456789".ToCharArray().Any(i => str.Contains(i))) return str;
        str = str[..str.IndexOfAny("0123456789".ToCharArray())];
        if (str.EndsWith("-") || str.EndsWith("_")) return str[..^1];
        return str;
    }

    public static bool SameName(this string str, string str2)
    {
        return str.PureName().Equals(str2.PureName(), StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool ContainsName(this IEnumerable<string> arr, string name)
    {
        return arr.Any(n => n.SameName(name));
    }

    public static List<string> Logs { get; set; } = new();
    public static void Log(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Logs.Add(text);
    }
    public static string GetPath()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new PathNotFoundException();
        if (path.Contains("bin")) path = path[..path.IndexOf(@"\bin")] ?? throw new PathNotFoundException();
        return path;
    }
}
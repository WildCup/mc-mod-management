namespace McHelper.Extensions;

public static class Logger
{
	public static void Log(string msg, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.Write(msg);
		Console.ForegroundColor = ConsoleColor.White;
	}
	public static void LogLine(string msg, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.WriteLine(msg);
		Console.ForegroundColor = ConsoleColor.White;
	}

	public static void Log(string msg) => Console.Write(msg);
	public static void LogLine(string msg) => Console.WriteLine(msg);
}

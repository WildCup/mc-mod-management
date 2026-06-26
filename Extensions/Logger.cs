namespace McHelper.Extensions;

public readonly record struct LogEntry(string Message, ConsoleColor Color);

public static class Logger
{
	/// <summary> Raised for every logged line/segment so the UI can mirror console output. </summary>
	public static event Action<LogEntry>? OnLog;

	public static void Log(string msg, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.Write(msg);
		Console.ForegroundColor = ConsoleColor.White;
		OnLog?.Invoke(new LogEntry(msg, color));
	}

	public static void LogLine(string msg, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.WriteLine(msg);
		Console.ForegroundColor = ConsoleColor.White;
		OnLog?.Invoke(new LogEntry(msg + "\n", color));
	}

	public static void Log(string msg)
	{
		Console.Write(msg);
		OnLog?.Invoke(new LogEntry(msg, ConsoleColor.White));
	}

	public static void LogLine(string msg)
	{
		Console.WriteLine(msg);
		OnLog?.Invoke(new LogEntry(msg + "\n", ConsoleColor.White));
	}
}

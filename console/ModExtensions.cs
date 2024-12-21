public class ModExtensions
{
	public static void Log(string msg, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.WriteLine(msg);
		Console.ForegroundColor = ConsoleColor.White;
	}
}

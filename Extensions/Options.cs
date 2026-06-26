namespace McHelper.Extensions;

public class Options
{
	private static readonly string _basePath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
	public static readonly string InputBase = "~/.minecraft";
	public static readonly string InputPath = $"{InputBase}/mods";
	public static readonly string ModsPath = $"{_basePath}/Db/installed.json";
	public static readonly string KnownPath = $"{_basePath}/Db/known.json";
	public static readonly string BackupPath = $"{_basePath}/Db/backup.json";
	public static readonly string[] Ignore = [];
}

namespace McHelper.Extensions;

/// <summary> Bound from appsettings.json / user-secrets </summary>
public class Config
{
	/// <summary> Root of the local minecraft install </summary>
	public string MinecraftPath { get; set; } = "~/.minecraft";

	/// <summary> How to connect to the server </summary>
	public required FtpConfig Ftp { get; init; }
}

/// <summary>
/// How to connect to the server.
/// Contains server address, credential etc.
/// TODO: support other providers? will this work with azure VM?
/// </summary>
public class FtpConfig
{
	public required string Url { get; init; }
	public required int Port { get; init; }
	public required string Username { get; init; }
	public required string Password { get; init; }

	/// <summary> Path to folder with Minecraft server on the server machine </summary>
	public required string BasePath { get; init; }
}

/// <summary>
/// Absolute filesystem paths references.
/// </summary>
public class Paths(Config config, string contentRoot)
{
	private readonly string _dbDir = Path.Combine(contentRoot, "Db");

	/// <summary> Path to .minecraft folder </summary>
	public string McRoot => Expand(config.MinecraftPath);

	/// <summary> Path to .minecraft/mods </summary>
	public string McLocalMods => Path.Combine(McRoot, "mods");

	/// <summary> Path to mods.json (database) </summary>
	public string KnownMods => Path.Combine(_dbDir, "mods.json");

	/// <summary> Path to backup.json (database backup) </summary>
	public string Backup => Path.Combine(_dbDir, "backup.json");

	private static string Expand(string path)
	{
		return path.StartsWith('~')
			? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + path[1..]
			: path;
	}
}

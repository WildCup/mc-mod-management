namespace McHelper.Extensions;

/// <summary> Bound from appsettings.json / user-secrets </summary>
public class Config
{
	/// <summary> Root of the local minecraft install </summary>
	public string MinecraftPath { get; set; } = "~/.minecraft";

	/// <summary> FTP server connection, if one is configured. </summary>
	public FtpConfig? Ftp { get; init; }

	/// <summary> SFTP-over-SSH server connection, if one is configured. </summary>
	public SftpConfig? Sftp { get; init; }

	/// <summary> Whichever server is configured. SFTP wins if both sections are present. </summary>
	public ServerConfig Server => (ServerConfig?)Sftp ?? Ftp
		?? throw new YouIdiotException("No server configured — add either App:Sftp or App:Ftp to appsettings.");
}

/// <summary> How to reach the Minecraft server. The concrete subclass decides the transport. </summary>
public abstract class ServerConfig
{
	public required string Host { get; init; }
	public required int Port { get; init; }
	public required string Username { get; init; }

	/// <summary> Minecraft server folder on the host (absolute, or relative to the login directory). </summary>
	public required string BasePath { get; init; }

	/// <summary> Human-readable transport name for the UI ("FTP" / "SFTP"). </summary>
	public abstract string Kind { get; }

	/// <summary> Enough is filled in to attempt a connection. </summary>
	public bool Ready => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(BasePath);
}

/// <summary> Plain FTP (e.g. shared hosts like craftserve). </summary>
public sealed class FtpConfig : ServerConfig
{
	public required string Password { get; init; }
	public override string Kind => "FTP";
}

/// <summary>
/// SFTP over SSH — works on a stock Azure VM with no extra setup, reusing the same SSH login.
/// Uses key auth when <see cref="KeyPath"/> is set, otherwise password auth.
/// </summary>
public sealed class SftpConfig : ServerConfig
{
	/// <summary> Path to the private SSH key, e.g. ~/.ssh/id_ed25519. </summary>
	public string? KeyPath { get; init; }

	/// <summary> Key passphrase when KeyPath is set, otherwise the account password. </summary>
	public string? Password { get; init; }

	public override string Kind => "SFTP";
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

	/// <summary> Expand a leading "~" to the user's home directory. </summary>
	public static string Expand(string path)
	{
		return path.StartsWith('~')
			? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + path[1..]
			: path;
	}
}

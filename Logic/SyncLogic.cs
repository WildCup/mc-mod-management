using System.Net;
using System.Text;
using FluentFTP;
using McHelper.Domain.Models;
using McHelper.Extensions;
using FtpConfig = McHelper.Extensions.FtpConfig;

namespace McHelper.Logic;

/// <summary>
/// Responsible for uploading all relevant files to the server via FTP
/// </summary>
public sealed class SyncLogic(FtpConnector connector, FtpConfig config, Paths paths)
{
	public void SyncMods(IEnumerable<Mod> installedMods, bool upload = false)
	{
		Logger.LogLine("\nSynchronizing mods", ConsoleColor.Magenta);

		var localFiles = installedMods
			.Where(m => m.Category != Category.Client)
			.Select(m => m.FileName)
			.Where(f => !string.IsNullOrEmpty(f))
			.Order()
			.ToArray();

		var from = Path.Combine(paths.McRoot, "mods");
		var to = Path.Combine(config.BasePath, "mods");
		connector.TransferSelected(from, to, localFiles, upload);
	}

	public void SyncConfig(bool upload = false)
	{
		Logger.LogLine("\nSynchronizing config", ConsoleColor.Magenta);
		var from = Path.Combine(paths.McRoot, "config");
		var to = Path.Combine(config.BasePath, "config");
		connector.TransferAll(from, to, upload);
	}

	public void SyncDatapacks(bool upload = false)
	{
		Logger.LogLine("\nSynchronizing datapacks", ConsoleColor.Magenta);
		var from = Path.Combine(paths.McRoot, "saves/final1/datapacks");
		var to = Path.Combine(config.BasePath, "world/datapacks");
		connector.TransferAll(from, to, upload);
	}

	public void SyncServerConfig(bool upload = false)
	{
		Logger.LogLine("\nSynchronizing serverconfig", ConsoleColor.Magenta);
		var from = Path.Combine(paths.McRoot, "saves/final1/serverconfig");
		var to = Path.Combine(config.BasePath, "world/serverconfig");
		connector.TransferAll(from, to, upload);
	}
}

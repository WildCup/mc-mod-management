using McHelper.Domain.Models;
using McHelper.Extensions;

namespace McHelper.Logic;

/// <summary>
/// In-memory db state and the actions the UI triggers.
/// Single db model: "installed" is observed live from the mods folder, never stored.
/// </summary>
public class ModService(ModsRepo repo, FtpConfig ftpConfig, Paths paths)
{
	public List<Mod> Mods { get; private set; } = [];

	/// <summary> Load db, scan folder, flag installed, add newly found files. </summary>
	public void Refresh()
	{
		Mods = repo.Load();
		var installed = repo.ScanInstalled();

		foreach (var name in installed)
		{
			if (!Mods.Any(m => m.IsSameMod(name)))
			{
				Mods.Add(new Mod { Name = name });
				Logger.LogLine($"New mod found: {name}", ConsoleColor.Green);
			}
		}

		var installedSet = installed.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
		foreach (var mod in Mods)
			mod.Installed = installedSet.Contains(mod.Name);

		Logger.LogLine($"DB: {Mods.Count} mods, {installedSet.Count} installed", ConsoleColor.Magenta);
	}

	public void Save() => repo.Save(Mods);

	public void Delete(Mod mod)
	{
		_ = Mods.Remove(mod);
		Logger.LogLine($"Removed {mod.Name} from db", ConsoleColor.Red);
	}

	private IEnumerable<Mod> Installed => Mods.Where(m => m.Installed);

	public void FtpMods(bool upload) => WithFtp(ftp => ftp.SyncMods(Installed, upload));
	public void FtpConfigFiles(bool upload) => WithFtp(ftp => ftp.SyncConfig(upload));
	public void FtpDatapacks(bool upload) => WithFtp(ftp => ftp.SyncDatapacks(upload));
	public void FtpServerConfig(bool upload) => WithFtp(ftp => ftp.SyncServerConfig(upload));

	private void WithFtp(Action<SyncLogic> action)
	{
		try
		{
			using var ftpConnector = new FtpConnector(ftpConfig);
			var ftp = new SyncLogic(ftpConnector, ftpConfig, paths);
			action(ftp);
		}
		catch (Exception ex)
		{
			Logger.LogLine($"FTP failed: {ex.Message}", ConsoleColor.Red);
		}
	}
}

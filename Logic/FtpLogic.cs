using System.Net;
using System.Text;
using FluentFTP;
using McHelper.Domain.Models;
using McHelper.Extensions;

namespace McHelper.Logic;

/// <summary>
/// Responsible for uploading mods to server via FTP
/// TODO: move configs to appsettings
/// allow sync with different providers then craftserve
/// </summary>
public class FtpLogic
{
	private readonly FtpClient _client;
	private readonly string _username = "{value}";
	private readonly string _password = "{value}";
	private readonly string _url = "ftp://ftp.craftserve.pl";
	private readonly string _basePath = "{root-folder-with-the-server-files}";
	private readonly string[] _exclude = ["oculus-mc1.19.2-1.6.9.jar"];

	public FtpLogic()
	{
		_client = new FtpClient(_url, 21);
		_client.Connect(new FtpProfile()
		{
			Host = _url,
			Credentials = new NetworkCredential(_username, _password),
			Encoding = Encoding.Default,
		});
	}

	public void Sync(IEnumerable<Mod> inputMods, IEnumerable<Mod> mods, bool forSure = false)
	{
		Logger.LogLine("\nSynchronizing mods", ConsoleColor.Magenta);
		var localFiles = GetLocalFiles(inputMods, mods).ToArray();

		var from = $"{Options.InputBase}/mods";
		var to = $"{_basePath}/mods";

		if (forSure)
			UploadFiles(from, to, localFiles);
		else
			Diff(from, to);
	}

	private List<string> GetLocalFiles(IEnumerable<Mod> inputMods, IEnumerable<Mod> mods)
	{
		var toReturn = new List<string>();
		foreach (var mod in inputMods)
		{
			var mapped = mods.FirstOrDefault(x => x.IsSameMod(mod));
			mapped ??= mods.FirstOrDefault(x => x.Dependencies.Contains(mod.Name) && x.Category != Category.Client);
			mapped ??= mods.First(x => x.Dependencies.Contains(mod.Name));

			if (mapped.Category == Category.Client)
				continue;

			if (_exclude.Contains(mod.Name))
				continue;

			toReturn.Add(mod.Name);
		}

		return toReturn;
	}

	public void SyncConfig(bool forSure = false)
	{
		Logger.LogLine("\nSynchronizing config", ConsoleColor.Magenta);

		var from = $"{Options.InputBase}/config";
		var to = $"{_basePath}/config";

		if (forSure)
			UploadFiles(from, to);
		else
			Diff(from, to);
	}

	public void SyncDatapacks(bool forSure = false)
	{
		Logger.LogLine("\nSynchronizing datapacks", ConsoleColor.Magenta);

		var from = $"{Options.InputBase}/saves/final1/datapacks";
		var to = $"{_basePath}/world/datapacks";

		if (forSure)
			UploadFiles(from, to);
		else
			Diff(from, to);
	}

	public void SyncServerConfig(bool forSure = false)
	{
		Logger.LogLine("\nSynchronizing serverconfig", ConsoleColor.Magenta);

		var from = $"{Options.InputBase}/saves/final1/serverconfig";
		var to = $"{_basePath}/world/serverconfig";

		if (forSure)
			UploadFiles(from, to);
		else
			Diff(from, to);
	}

	private void UploadFiles(string from, string to, string[]? localFiles = null)
	{
		Logger.LogLine($"uploading files from: {from} to: {to}");

		localFiles ??= GetLocalFiles(from);
		var remoteFiles = GetRemoteFiles(to);

		Logger.LogLine($"local files: {localFiles.Length} remote files: {remoteFiles.Count()}");

		var toUpload = localFiles.Where(x => !remoteFiles.Contains(x));
		var toRemove = remoteFiles.Where(x => !localFiles.Contains(x));

		if (!toUpload.Any() && !toRemove.Any())
		{
			Logger.LogLine("no differences");
			return;
		}

		if (toUpload.Any())
			Logger.LogLine("\nuploading to server:");

		foreach (var file in toUpload)
		{
			var result = _client.UploadFile($"{from}/{file}", $"{to}/{file}", FtpRemoteExists.Skip);
			Logger.LogLine(file + " uploaded " + result ?? "", ConsoleColor.DarkGreen);
		}

		if (toRemove.Any())
			Logger.LogLine("\ndeleting from server:");

		foreach (var file in toRemove)
		{
			_client.DeleteFile($"{to}/{file}");
			Logger.LogLine(file + " deleted", ConsoleColor.DarkRed);
		}
	}

	private void Diff(string from, string to, string[]? localFiles = null)
	{
		Logger.LogLine($"comparing files from: {from} to: {to}");

		localFiles ??= GetLocalFiles(from);
		var remoteFiles = GetRemoteFiles(to);

		Logger.LogLine($"local files: {localFiles.Length} remote files: {remoteFiles.Count()}");

		var toUpload = localFiles.Where(x => !remoteFiles.Contains(x));
		var toRemove = remoteFiles.Where(x => !localFiles.Contains(x));

		if (!toUpload.Any() && !toRemove.Any())
		{
			Logger.LogLine("no differences");
			return;
		}

		if (toUpload.Any())
			Logger.LogLine("\nlocally but not on the server:");

		foreach (var file in toUpload)
			Logger.LogLine(file, ConsoleColor.DarkGreen);

		if (toRemove.Any())
			Logger.LogLine("\non the server but not locally:");

		foreach (var file in toRemove)
			Logger.LogLine(file, ConsoleColor.DarkRed);
	}

	private IEnumerable<string> GetRemoteFiles(string path, string? currentPath = null)
	{
		var result = new List<string>();

		// Get all items in the current directory
		foreach (var item in _client.GetListing(currentPath ?? path))
		{
			if (item.Type == FtpObjectType.File)
			{
				var relativePath = item.FullName[(path.Length + 2)..];
				result.Add(relativePath);
			}
			else if (item.Type == FtpObjectType.Directory)
			{
				result.AddRange(GetRemoteFiles(path, item.FullName));
			}
		}

		return result.Order();
	}

	private static string[] GetLocalFiles(string path)
	{
		return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
			.Select(x => Path.GetRelativePath(path, x) ?? "")
			.Order()
			.ToArray();
	}
}


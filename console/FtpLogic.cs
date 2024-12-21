namespace McHelper;

using System.Net;
using System.Text;
using FluentFTP;
using McHelper.Domain.Models.Enums;

public class FtpLogic
{
	private readonly FtpClient _client;
	private readonly string _username = "hagelhubert@gmail.com";
	private readonly string _password = "instanceS16x@";
	private readonly string _url = "ftp://ftp.craftserve.pl";
	private readonly string _path = "1000035/mods";
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

	public IEnumerable<string> GetFilesFromServer()
	{
		return _client.GetNameListing(_path).Order();
	}

	public void Sync(IEnumerable<Mod> inputMods, IEnumerable<Mod> mods, bool forSure = false)
	{
		var localFiles = GetLocalFiles(inputMods, mods);
		var remoteFiles = GetFilesFromServer();

		var toUpload = localFiles.Where(x => !remoteFiles.Contains(x));

		ModExtensions.Log($"local files: {localFiles.Count}\tremote files: {remoteFiles.Count()}", ConsoleColor.Magenta);

		if (toUpload.Any())
			Console.WriteLine("\nUploading to server:");
		foreach (var file in toUpload)
		{
			FtpStatus? result = null;
			if (forSure)
				result = _client.UploadFile($"{Options.InputPath}/{file}", $"{_path}/{file}", FtpRemoteExists.Skip);
			ModExtensions.Log(file + " uploaded " + result ?? "", ConsoleColor.DarkGreen);
		}

		var toRemove = remoteFiles
			.Where(x => !localFiles.Contains(x));

		if (toRemove.Any())
			Console.WriteLine("\nDeleting from server:");
		foreach (var file in toRemove)
		{
			if (forSure)
				_client.DeleteFile($"{_path}/{file}");
			ModExtensions.Log(file + " deleted", ConsoleColor.DarkRed);
		}
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

	public void CompareConfig()
	{
		var localFiles = Directory.GetFiles(@"/home/puffin/.minecraft/config").Select(x => Path.GetFileName(x) ?? "").ToArray();
		var remoteFiles = _client.GetNameListing("1000035/config").Order();

		var toUpload = localFiles.Where(x => !remoteFiles.Contains(x));

		ModExtensions.Log($"local configs: {localFiles.Length}\tremote: {remoteFiles.Count()}", ConsoleColor.Magenta);

		Console.WriteLine("\nLocally but not on the server:");
		foreach (var file in toUpload)
			ModExtensions.Log(file + " uploaded", ConsoleColor.DarkGreen);

		var toRemove = remoteFiles.Where(x => !localFiles.Contains(x));

		Console.WriteLine("\nOn the server but not locally:");
		foreach (var file in toRemove)
			ModExtensions.Log(file + " deleted", ConsoleColor.DarkRed);
	}
}


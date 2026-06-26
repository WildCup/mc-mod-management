using System.Net;
using System.Text;
using FluentFTP;
using McHelper.Extensions;
using FtpConfig = McHelper.Extensions.FtpConfig;

namespace McHelper.Logic;

/// <summary>
/// Responsible for uploading files to the server via FTP.
/// Use upload = false to preview diff before uploading
/// </summary>
public sealed class FtpConnector : IDisposable
{
	private readonly FtpClient _client;

	public FtpConnector(FtpConfig config)
	{
		_client = new(config.Url, config.Port);
		_client.Connect(new FtpProfile()
		{
			Host = config.Url,
			Credentials = new NetworkCredential(config.Username, config.Password),
			Encoding = Encoding.Default,
		});
	}

	#region transfer
	/// <summary> Transfer all files from local PC to remote server (or preview the diff only) </summary>
	public void TransferAll(string from, string to, bool upload) => Transfer(from, to, null, upload);

	/// <summary> Transfer selected files from local PC to remote server (or preview the diff only) </summary>
	public void TransferSelected(string from, string to, string[] fileNames, bool upload) => Transfer(from, to, fileNames, upload);

	//transfer files from local PC to remote server (or preview the diff only)
	private void Transfer(string from, string to, string[]? fileNames, bool upload)
	{
		var log = upload ? "" : " (diff only)";
		Logger.LogLine($"transferring from: {from} to: {to}" + log);

		var localFiles = GetLocalFiles(from, fileNames);
		var remoteFiles = GetRemoteFiles(to);

		var toUpload = localFiles.Where(x => !remoteFiles.Contains(x)).ToArray();
		var toRemove = remoteFiles.Where(x => !localFiles.Contains(x)).ToArray();

		if (toUpload.Length == 0 && toRemove.Length == 0)
		{
			Logger.LogLine("no differences");
			return;
		}

		if (upload)
			UploadFiles(from, to, toUpload, toRemove);
		else
			Diff(toUpload, toRemove);
	}
	#endregion

	#region diff
	//upload/remove files to the server
	private void UploadFiles(string from, string to, string[] toUpload, string[] toRemove)
	{
		//upload
		if (toUpload.Length != 0)
			Logger.LogLine("\nuploading to server:");
		foreach (var file in toUpload)
		{
			var result = _client.UploadFile($"{from}/{file}", $"{to}/{file}", FtpRemoteExists.Skip);
			Logger.LogLine(file + " " + result, ConsoleColor.DarkGreen);
		}

		//remove
		if (toRemove.Length != 0)
			Logger.LogLine("\ndeleting from server:");
		foreach (var file in toRemove)
		{
			_client.DeleteFile($"{to}/{file}");
			Logger.LogLine(file, ConsoleColor.DarkRed);
		}
	}

	//print diff only without doing anything
	private void Diff(string[] toUpload, string[] toRemove)
	{
		if (toUpload.Length != 0)
			Logger.LogLine("\nlocally but not on the server:");

		foreach (var file in toUpload)
			Logger.LogLine(file, ConsoleColor.DarkGreen);

		if (toRemove.Length != 0)
			Logger.LogLine("\non the server but not locally:");

		foreach (var file in toRemove)
			Logger.LogLine(file, ConsoleColor.DarkRed);
	}
	#endregion

	#region get
	//returns remote files names relative to path
	//path/
	//├── subpath/
	//│   └── item1    =>    subpath/item1, item2
	//└── item2
	private IEnumerable<string> GetRemoteFiles(string path)
	{
		Logger.LogLine($"loading remote files: {path}");

		var fullNames = GetRemoteFullFiles(path);
		var result = fullNames
			.Select(x => Path.GetRelativePath(path, x))
			.Order()
			.ToArray();

		Logger.LogLine($"found {result.Length}");
		return result;
	}

	//returns all files recursively from remote/path - full names
	private List<string> GetRemoteFullFiles(string path)
	{
		var result = new List<string>();

		// Get all items in the current directory
		foreach (var item in _client.GetListing(path))
		{
			if (item.Type == FtpObjectType.File)
				result.Add(item.FullName);
			else if (item.Type == FtpObjectType.Directory)
				result.AddRange(GetRemoteFullFiles(item.FullName));
		}

		return result;
	}

	//returns local files names relative to path
	//path/
	//├── subpath/
	//│   └── item1    =>    subpath/item1, item2
	//└── item2
	private static string[] GetLocalFiles(string path, string[]? fileNames = null)
	{
		Logger.LogLine($"loading local files: {path}");

		var all = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
			.Select(x => Path.GetRelativePath(path, x));

		if (fileNames != null)
			all = all.Where(x => fileNames.Contains(x));

		var result = all.Order().ToArray();

		Logger.LogLine($"found {result.Length}");
		return result;
	}
	#endregion

	public void Dispose() => _client?.Dispose();
}

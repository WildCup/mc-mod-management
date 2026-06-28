using System.Net;
using System.Text;
using FluentFTP;
using FluentFTP.Exceptions;
using FtpConfig = McHelper.Extensions.FtpConfig;

namespace McHelper.Logic;

/// <summary> Plain FTP transport backed by FluentFTP. </summary>
public sealed class FtpTransport : ITransport
{
	private readonly FtpClient _client;

	public FtpTransport(FtpConfig config)
	{
		_client = new(config.Host, config.Port);
		_client.Config.ConnectTimeout = 15_000;
		_client.Connect(new FtpProfile()
		{
			Host = config.Host,
			Credentials = new NetworkCredential(config.Username, config.Password),
			Encoding = Encoding.Default,
		});
	}

	public IReadOnlyDictionary<string, long> List(string remoteDir)
	{
		var result = new Dictionary<string, long>(StringComparer.Ordinal);
		try
		{
			Walk(remoteDir, "", result);
		}
		catch (FtpException)
		{
			// directory does not exist yet on the server — treat as empty
		}
		return result;
	}

	//key files by their path relative to the listed dir, built from names — item.FullName is server-absolute
	private void Walk(string dir, string prefix, Dictionary<string, long> acc)
	{
		foreach (var item in _client.GetListing(dir))
		{
			var rel = prefix.Length == 0 ? item.Name : $"{prefix}/{item.Name}";
			if (item.Type == FtpObjectType.File)
				acc[rel] = item.Size;
			else if (item.Type == FtpObjectType.Directory)
				Walk(item.FullName, rel, acc);
		}
	}

	public IReadOnlyList<string> ListDirs(string remoteDir)
	{
		try
		{
			return _client.GetListing(remoteDir)
				.Where(i => i.Type == FtpObjectType.Directory)
				.Select(i => i.Name)
				.ToList();
		}
		catch (FtpException)
		{
			return [];
		}
	}

	public void Upload(string localFile, string remoteFile) =>
		_client.UploadFile(localFile, remoteFile, FtpRemoteExists.Overwrite, createRemoteDir: true);

	public void Delete(string remoteFile) => _client.DeleteFile(remoteFile);

	public void Dispose() => _client?.Dispose();
}

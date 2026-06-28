using McHelper.Extensions;
using Renci.SshNet;

namespace McHelper.Logic;

/// <summary>
/// SFTP transport over SSH. Works on a stock Azure Linux VM with nothing extra installed:
/// it reuses the same SSH port (22) and key the user already logs in with — no FTP server,
/// no extra firewall/NSG rules. Auth is by private key when <see cref="SftpConfig.KeyPath"/> is set,
/// otherwise by password.
///
/// Listing is done with a single `find` command per folder instead of a recursive SFTP walk:
/// a config tree with dozens of subdirectories would otherwise cost one network round-trip per
/// directory. The SFTP file-transfer channel is only opened lazily, when something is uploaded.
/// </summary>
public sealed class SftpTransport : ITransport
{
	private readonly SftpConfig _config;
	private readonly SshClient _ssh;
	private SftpClient? _sftp;

	public SftpTransport(SftpConfig config)
	{
		_config = config;
		_ssh = new SshClient(SshConnection.Build(config));
		_ssh.Connect();
	}

	//opened only when we actually transfer files — a pure diff never needs it
	private SftpClient Sftp()
	{
		if (_sftp is null)
		{
			_sftp = new SftpClient(SshConnection.Build(_config));
			_sftp.Connect();
		}
		return _sftp;
	}

	public IReadOnlyDictionary<string, long> List(string remoteDir)
	{
		// one round-trip: find prints "<size>\t<relative-path>" for every file under remoteDir
		var output = Run($"cd {Quote(remoteDir)} 2>/dev/null && find . -type f -printf '%s\\t%P\\n'");
		var result = new Dictionary<string, long>(StringComparer.Ordinal);
		foreach (var line in output.Split('\n'))
		{
			var tab = line.IndexOf('\t');
			if (tab <= 0)
				continue;
			if (long.TryParse(line[..tab], out var size))
				result[line[(tab + 1)..]] = size;
		}
		return result;
	}

	public IReadOnlyList<string> ListDirs(string remoteDir)
	{
		var output = Run($"find {Quote(remoteDir)} -mindepth 1 -maxdepth 1 -type d -printf '%f\\n' 2>/dev/null");
		return output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
	}

	public void Upload(string localFile, string remoteFile)
	{
		var sftp = Sftp();
		EnsureDir(sftp, ParentDir(remoteFile));
		using var stream = File.OpenRead(localFile);
		sftp.UploadFile(stream, remoteFile, canOverride: true);
	}

	public void Delete(string remoteFile)
	{
		var sftp = Sftp();
		if (sftp.Exists(remoteFile))
			sftp.DeleteFile(remoteFile);
	}

	private string Run(string command)
	{
		using var cmd = _ssh.CreateCommand(command);
		return cmd.Execute();
	}

	//SSH.NET does not auto-create parent directories, so walk up and create the missing ones
	private static void EnsureDir(SftpClient sftp, string dir)
	{
		if (string.IsNullOrEmpty(dir) || dir == "/" || sftp.Exists(dir))
			return;
		EnsureDir(sftp, ParentDir(dir));
		sftp.CreateDirectory(dir);
	}

	private static string ParentDir(string path)
	{
		var i = path.TrimEnd('/').LastIndexOf('/');
		return i <= 0 ? "" : path[..i];
	}

	//wrap in single quotes for the remote /bin/sh, escaping any embedded single quotes
	private static string Quote(string value) => $"'{value.Replace("'", "'\\''")}'";

	public void Dispose()
	{
		if (_sftp is not null)
		{
			if (_sftp.IsConnected) _sftp.Disconnect();
			_sftp.Dispose();
		}
		if (_ssh.IsConnected) _ssh.Disconnect();
		_ssh.Dispose();
	}
}

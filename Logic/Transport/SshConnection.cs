using McHelper.Extensions;
using Renci.SshNet;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace McHelper.Logic;

/// <summary> Builds SSH connection info from an <see cref="SftpConfig"/> (key auth when a key is set, else password). </summary>
public static class SshConnection
{
	public static ConnectionInfo Build(SftpConfig config)
	{
		var info = config.KeyPath is { Length: > 0 }
			? new ConnectionInfo(config.Host, config.Port, config.Username,
				new PrivateKeyAuthenticationMethod(config.Username, LoadKey(config)))
			: new ConnectionInfo(config.Host, config.Port, config.Username,
				new PasswordAuthenticationMethod(config.Username, config.Password));
		info.Timeout = TimeSpan.FromSeconds(15);
		return info;
	}

	private static PrivateKeyFile LoadKey(SftpConfig config)
	{
		var path = Paths.Expand(config.KeyPath!);
		return string.IsNullOrEmpty(config.Password)
			? new PrivateKeyFile(path)
			: new PrivateKeyFile(path, config.Password);
	}
}

using McHelper.Extensions;

namespace McHelper.Logic;

/// <summary> Opens the right <see cref="ITransport"/> for a configured server. </summary>
public static class TransportFactory
{
	public static ITransport Connect(ServerConfig config) => config switch
	{
		SftpConfig sftp => new SftpTransport(sftp),
		FtpConfig ftp => new FtpTransport(ftp),
		_ => throw new YouIdiotException($"Unsupported server config: {config.GetType().Name}"),
	};
}

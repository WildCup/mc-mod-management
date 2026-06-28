namespace McHelper.Logic;

/// <summary>
/// A connection to the server that can list, upload and delete files.
/// Implemented by <see cref="FtpTransport"/> (plain FTP) and <see cref="SftpTransport"/> (SFTP over SSH).
/// </summary>
public interface ITransport : IDisposable
{
	/// <summary>
	/// All files under <paramref name="remoteDir"/> recursively, keyed by their path relative to it
	/// (forward slashes), with their size in bytes. Returns empty if the directory does not exist.
	/// </summary>
	IReadOnlyDictionary<string, long> List(string remoteDir);

	/// <summary> Immediate subdirectory names directly under <paramref name="remoteDir"/>; empty if it does not exist. </summary>
	IReadOnlyList<string> ListDirs(string remoteDir);

	/// <summary> Upload a local file to the remote path, overwriting and creating parent dirs as needed. </summary>
	void Upload(string localFile, string remoteFile);

	/// <summary> Delete a remote file if it exists. </summary>
	void Delete(string remoteFile);
}

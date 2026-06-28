using McHelper.Extensions;
using Renci.SshNet;

namespace McHelper.Logic;

/// <summary> Result of a shell command run over SSH. </summary>
public sealed record CommandResult(string Output, string Error, int ExitStatus)
{
	public bool Ok => ExitStatus == 0;
	public string Combined => string.IsNullOrEmpty(Error) ? Output : $"{Output}{Error}";
}

public enum ServerState { Unknown, Running, Stopped }

/// <summary> A single poll of the server: its state plus the tail of its log. </summary>
public sealed record ServerSnapshot(ServerState State, string Log);

/// <summary>
/// Starts/stops the Minecraft server and runs commands on the host over SSH.
/// The server runs inside a detached tmux session named "mc" so its console stays attachable —
/// that's how we send console commands (tmux send-keys) and a graceful "stop".
/// Only works when the configured server is SFTP/SSH (not plain FTP).
/// </summary>
public sealed class ServerControl(ServerConfig server)
{
	private const string Session = "mc";

	/// <summary> Server control is only possible over SSH. </summary>
	public bool Available => server is SftpConfig;

	private SftpConfig Cfg => server as SftpConfig
		?? throw new YouIdiotException("Server control needs an SFTP (SSH) server, not FTP.");

	//the server dir, single-quoted for safe use inside a remote shell (e.g. "'mc-server'")
	private string Dir => Quote(Cfg.BasePath);

	/// <summary> Start the server in a detached tmux session if it isn't already running. </summary>
	public CommandResult Start() => Exec(
		$"tmux has-session -t {Session} 2>/dev/null && echo 'already running' || " +
		$"tmux new-session -d -s {Session} 'cd {Dir} && ./run.sh nogui'");

	/// <summary> Ask the server console to stop gracefully. </summary>
	public CommandResult Stop() => SendConsole("stop");

	/// <summary> Force-kill the tmux session (use only if a graceful stop hangs). </summary>
	public CommandResult Kill() => Exec($"tmux kill-session -t {Session} 2>/dev/null; echo killed");

	/// <summary> Send a command to the running server console (no leading slash needed). </summary>
	public CommandResult SendConsole(string command) =>
		Exec($"tmux send-keys -t {Session} {Quote(command)} Enter");

	/// <summary> State + log tail in a single SSH round-trip (for polling). </summary>
	public ServerSnapshot Snapshot(int lines = 200)
	{
		const string end = "@@LOG@@";
		var r = Exec(
			$"(tmux has-session -t {Session} 2>/dev/null && echo running || echo stopped); " +
			$"echo {end}; tail -n {lines} {Dir}/logs/latest.log 2>/dev/null");

		if (!r.Ok && string.IsNullOrWhiteSpace(r.Output))
			return new(ServerState.Unknown, r.Error);

		var split = r.Output.IndexOf(end, StringComparison.Ordinal);
		var head = split < 0 ? r.Output : r.Output[..split];
		var log = split < 0 ? "" : r.Output[(split + end.Length)..].TrimStart('\r', '\n');
		var state = head.Contains("running") ? ServerState.Running : ServerState.Stopped;
		return new(state, log);
	}

	private CommandResult Exec(string command)
	{
		using var client = new SshClient(SshConnection.Build(Cfg));
		client.Connect();
		using var cmd = client.CreateCommand(command);
		var output = cmd.Execute();
		return new(output, cmd.Error, cmd.ExitStatus ?? -1);
	}

	//wrap in single quotes for the remote /bin/sh, escaping any embedded single quotes
	private static string Quote(string value) => $"'{value.Replace("'", "'\\''")}'";
}

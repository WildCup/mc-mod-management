namespace McHelper.Tests;
/*
using McHelper.Application.Logic;
using McHelper.Domain.Extensions;
using McHelper.Domain.Models;

public class SyncDependenciesTest
{
	private List<Mod> _mods =
	[
		new Mod() {
			Name = "a-1.0",
			Dependencies = ["xxx-1.0", "yyy-1.0"]
		},
		new Mod() { Name = "b-1.0" },
	];
	private List<string> _logs = [];

	[Fact]
	public void WhenAlreadyAdded()
	{
		Act(new() { Name = "a-1.0", Dependencies = ["xxx-1.0", "yyy-1.0"] }, new() { Name = "b-1.0" });

		var modA = _mods.First(m => m.Name == "a-1.0");
		var modB = _mods.First(m => m.Name == "b-1.0");
		Assert.True(modA.Dependencies.Count == 2 && modA.Dependencies.Contains("xxx-1.0") && modA.Dependencies.Contains("yyy-1.0"));
		Assert.True(modB.Dependencies.Count == 0);
	}

	[Fact]
	public void WhenAutoAddingDependency()
	{
		Act(new() { Name = "a-1.0", Dependencies = ["xxx-1.0", "yyy-1.0"] }, new() { Name = "b-1.0", Dependencies = new() { "zzz-1.0" } });

		var modA = _mods.First(m => m.Name == "a-1.0");
		var modB = _mods.First(m => m.Name == "b-1.0");
		Assert.True(modA.Dependencies.Count == 2 && modA.Dependencies.Contains("xxx-1.0") && modA.Dependencies.Contains("yyy-1.0"));
		Assert.True(modB.Dependencies.Count == 1 && modB.Dependencies.Contains("zzz-1.0"));
		Assert.Contains(_logs, l => l.Contains("added zzz-1.0 to"));
	}

	[Fact]
	public void WhenNotAdded()
	{
		Act(new() { Name = "a-1.0", Dependencies = ["xxx-1.0"] }, new() { Name = "b-1.0" });

		var modA = _mods.First(m => m.Name == "a-1.0");
		var modB = _mods.First(m => m.Name == "b-1.0");
		Assert.True(modA.Dependencies.Count == 2 && modA.Dependencies.Contains("xxx-1.0") && modA.Dependencies.Contains("yyy-1.0"));
		Assert.True(modB.Dependencies.Count == 0);
		Assert.Contains(_logs, l => l.Contains("added zzz-1.0 to"));
	}

	[Fact]
	public void WhenNotRequired()
	{
		Act(new() { Name = "a-1.0", Dependencies = ["xxx-1.0"] }, new() { Name = "b-1.0" });

		var modA = _mods.First(m => m.Name == "a-1.0");
		var modB = _mods.First(m => m.Name == "b-1.0");
		Assert.True(modA.Dependencies.Count == 2 && modA.Dependencies.Contains("xxx-1.0") && modA.Dependencies.Contains("yyy-1.0"));
		Assert.True(modB.Dependencies.Count == 0);
		Assert.Contains(_logs, l => l.Contains("yyy-1.0") && l.Contains("not required"));
	}

	[Fact]
	public void WhenUpdatedDependency()
	{
		Act(new() { Name = "a-1.0", Dependencies = ["xxx-1.0", "yyy-1.1"] }, new() { Name = "b-1.0" });

		var modA = _mods.First(m => m.Name == "a-1.0");
		var modB = _mods.First(m => m.Name == "b-1.0");
		Assert.True(modA.Dependencies.Count == 2 && modA.Dependencies.Contains("xxx-1.0") && modA.Dependencies.Contains("yyy-1.1"));
		Assert.True(modB.Dependencies.Count == 0);
		Assert.Contains(_logs, l => l.Contains("yyy-1.1 was updated"));
	}

	private void Act(params Mod[] modsInput)
	{
		var main = new Logic(_mods, []);
		main.Sync(modsInput);
		_mods = main.Mods.AsEnumerable().ToList();
		_logs = ModExtensions.Logs.AsEnumerable().ToList();
	}
}
*/

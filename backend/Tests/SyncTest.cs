namespace McHelper.Tests;

using McHelper.Application.Logic;
using McHelper.Domain.Extensions;
using McHelper.Domain.Models;


public class SyncTest
{
	private List<McMod> _mods =
	[
		new McMod() { Name = "a-1.0" },
		new McMod() {
			Name = "b-1.0" ,
			Priority = Priority.Necessary,
			WorksConfirmed = true,
			Category = Category.Look,
			Description = "test mod"
		},
	];
	private List<McMod> _modsKnown =
	[
		new McMod() {
			Name = "c-1.0" ,
			Priority = Priority.Necessary,
			WorksConfirmed = true,
			Category = Category.Look,
			Description = "test mod 2"
		},
	];
	private List<string> _logs = [];

	[Fact]
	public void WhenAlreadyAdded()
	{
		Act("a-1.0", "b-1.0");

		var mod = _mods.First(m => m.Name == "b-1.0");
		Assert.True(_mods.Count == 2);
		Assert.True(mod.Priority == Priority.Necessary);
		Assert.True(mod.WorksConfirmed);
		Assert.True(mod.Category == Category.Look);
		Assert.True(mod.Description == "test mod");
	}

	[Fact]
	public void WhenVersionChanged()
	{
		Act("a-1.0", "b-1.1");

		var mod = _mods.First(m => m.Name == "b-1.1");
		Assert.True(_mods.Count == 2);
		Assert.True(mod.Priority == Priority.Necessary);
		Assert.True(mod.WorksConfirmed);
		Assert.True(mod.Category == Category.Look);
		Assert.True(mod.Description == "test mod");

		Assert.Contains(_logs, l => l.Contains("b-1.1 was updated"));
	}

	[Fact]
	public void WhenMovedFromKnown()
	{
		Act("a-1.0", "b-1.0", "c-1.1");

		var mod = _mods.First(m => m.Name == "c-1.1");
		Assert.True(_mods.Count == 3);
		Assert.True(mod.Priority == Priority.Necessary);
		Assert.True(mod.WorksConfirmed);
		Assert.True(mod.Category == Category.Look);
		Assert.True(mod.Description == "test mod 2");

		Assert.Contains(_logs, l => l.Contains("c-1.1 was moved from known"));
	}

	[Fact]
	public void WhenAddingNew()
	{
		Act("a-1.0", "b-1.0", "x-1.0");

		var mod = _mods.First(m => m.Name == "x-1.0");
		Assert.True(_mods.Count == 3);
		Assert.True(mod.Priority == Priority.Unknown);
		Assert.True(mod.WorksConfirmed == false);
		Assert.True(mod.Category == Category.Unknown);
		Assert.True(mod.Description == "");

		Assert.Contains(_logs, l => l.Contains("x-1.0 was added"));
	}

	[Fact]
	public void WhenDeleting()
	{
		Act("b-1.0");

		Assert.True(_mods.Count == 1);
		Assert.True(_mods.FirstOrDefault(m => m.Name == "b-1.0") != null);

		Assert.True(_modsKnown.Count == 2);
		Assert.Contains(_logs, l => l.Contains("a-1.0 was deleted"));
	}

	private void Act(params string[] names)
	{
		var modsInput = new List<McMod>();
		foreach (var name in names)
			modsInput.Add(new McMod() { Name = name });

		var main = new Logic(_mods, _modsKnown);
		main.Sync(modsInput);
		_mods = main.Mods.AsEnumerable().ToList();
		_modsKnown = main.ModsKnown.AsEnumerable().ToList();
		_logs = ModExtensions.Logs.AsEnumerable().ToList();
	}
}

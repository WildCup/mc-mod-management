namespace McHelper.Tests;
/*
using McHelper.Application.Logic;
using McHelper.Domain.Extensions;
using McHelper.Domain.Models;

public class FileLogicIntegrationTest
{
	private readonly IReadOnlyCollection<McMod> _modsInput;

	private readonly List<string> _logs = [];

	[Fact]
	public void WhenUpdatedDependency()
	{
		var mod1 = _modsInput.First(m => m.Name == "AdvancementPlaques-1.19.2-1.4.7.jar");
		var mod2 = _modsInput.First(m => m.Name == "Botania-1.19.2-440-FORGE.jar");
		Assert.True(_modsInput.Count == 4);
		Assert.True(mod1.Dependencies.Count == 1 && mod1.Dependencies.Contains("Iceberg-1.19.2-forge-1.1.4.jar"));
		Assert.True(mod2.Dependencies.Count == 2 && mod2.Dependencies.Contains("Patchouli-1.19.2-77.jar"));

		Assert.Contains(_logs, log => log.Contains("curios required in Botania-1.19.2-440-FORGE.jar but not installed"));
	}

	public FileLogicIntegrationTest()
	{
		var path = ModExtensions.GetPath();
		var logic = new FileLogic(path + @"\Tests\ModsSample");

		_modsInput = logic.ModsInput;
		_logs = ModExtensions.Logs.AsEnumerable().ToList();
	}
}
*/

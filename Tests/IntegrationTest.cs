namespace McHelper.Tests;

public class IntegrationTest
{
    private List<Mod> _mods = new()
    {
        new Mod() {
            Name = "Xaeros_Minimap_23.6.0_Forge_1.11.5.jar" ,
            Priority = Priority.Necessary,
            WorksConfirmed = true,
            Category = Category.Convention,
            Description = "minimap"
        },
    };
    private List<Mod> _modsKnown = new()
    {
        new Mod() {
            Name = "Xaeros_Minimap_23.6.0_Forge_1.16.5.jar" ,
            Priority = Priority.Necessary,
            WorksConfirmed = true,
            Category = Category.Convention,
            Description = "minimap"
        },
    };
    private List<string> _logs = new();

    // [Fact]
    // public void WhenVersionChanged()
    // {
    //     Act("Xaeros_Minimap_23.6.0_Forge_1.16.5.jar");

    //     var mod = _mods.First(m => m.Name == "Xaeros_Minimap_23.6.0_Forge_1.16.5.jar");
    //     Assert.True(_mods.Count == 1);
    //     Assert.True(mod.Priority == Priority.Necessary);
    //     Assert.True(mod.WorksConfirmed == true);
    //     Assert.True(mod.Category == Category.Convention);
    //     Assert.True(mod.Description == "minimap");
    //     Assert.True(_logs.Where(l => l.Contains("updated")).Count() == 1);
    // }

    // private void Act(params string[] names)
    // {
    //     var main = new Logic(_mods, _modsKnown, _logs, names);
    //     main.Sync();
    //     _mods = main.Mods.AsEnumerable().ToList();
    //     _modsKnown = main.ModsKnown.AsEnumerable().ToList();
    //     _logs = main.Logs.AsEnumerable().ToList();
    // }
}

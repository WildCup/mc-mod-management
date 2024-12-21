namespace McHelper.Application.Logic;

public class Logic(IEnumerable<Mod> mods, IEnumerable<Mod> modsKnown)
{
	private List<Mod> _mods = mods.ToList();
	private List<Mod> _modsKnown = modsKnown.ToList();

	public IReadOnlyCollection<Mod> Mods => _mods;
	public IReadOnlyCollection<Mod> ModsKnown => _modsKnown;

	public void Sync(IEnumerable<Mod> modsInput)
	{
		ModExtensions.Log($"Synchronizing {modsInput.Count()} mods", ConsoleColor.Magenta);

		//sync - save new mods to known
		_modsKnown = _modsKnown.Where(b => !_mods.Any(m => m.IsSameMod(b))).ToList();
		_modsKnown.AddRange(_mods);

		//add - all mods, and dependencies if not added
		foreach (var modInput in modsInput)
		{
			SyncMod(modInput);
			SyncDependencies(modInput, modsInput);
		}

		//delete - delete mods that were not found in .minecraft/mods but are added
		var names = modsInput.Select(x => x.Name);
		var deleted = _mods.Where(m => !names.Contains(m.Name));
		foreach (var mod in deleted)
			ModExtensions.Log($"Mod {mod.Name} was deleted", ConsoleColor.Red);

		_mods = _mods.Except(deleted).ToList();

		ModExtensions.Log("Synchronization completed", ConsoleColor.Magenta);
	}

	private void SyncMod(Mod modInput)
	{
		//skip - already added
		var mod = _mods.FirstOrDefault(m => m.IsSameMod(modInput));
		if (mod != null)
			return;

		//ignore if not added and is a dependency
		mod = _mods.FirstOrDefault(m => m.Dependencies.Contains(modInput.Name));
		if (mod != null)
			return;

		//move from known and update - mod is not added but is known
		mod = _modsKnown.FirstOrDefault(m => m.IsSameMod(modInput));
		if (mod != null)
		{
			ModExtensions.Log($"Mod {modInput.Name} was moved from known {mod.Name}", ConsoleColor.Green);
			_mods.Add(mod);
			return;
		}

		//add
		ModExtensions.Log($"Mod {modInput.Name} was added", ConsoleColor.Green);
		_mods.Add(modInput);
		return;
	}

	private void SyncDependencies(Mod mod, IEnumerable<Mod> all)
	{
		//skip - already added
		foreach (var dependencyName in mod.Dependencies)
		{
			var dependencyMod = all.FirstOrDefault(m => m.IsSameMod(new Mod() { Name = dependencyName }));
			if (dependencyMod != null)
				continue;

			//remove not found dependency
			_ = mod.Dependencies.Remove(dependencyName);
			ModExtensions.Log($"Dependency {dependencyName} was deleted", ConsoleColor.DarkRed);
		}
	}
}

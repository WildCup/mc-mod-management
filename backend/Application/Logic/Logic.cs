namespace McHelper.Application.Logic;
/*
using McHelper.Domain.Extensions;
using McHelper.Domain.Models;

public class Logic(IEnumerable<McMod> mods, IEnumerable<McMod> modsKnown)
{
	private List<McMod> _mods = mods.ToList();
	private List<McMod> _modsKnown = modsKnown.ToList();

	public IReadOnlyCollection<McMod> Mods => new ReadOnlyCollection<McMod>(_mods);
	public IReadOnlyCollection<McMod> ModsKnown => new ReadOnlyCollection<McMod>(_modsKnown);

	public void Sync(IEnumerable<McMod> modsInput)
	{
		ModExtensions.Log($"Synchronizing {modsInput.Count()} mods", ConsoleColor.Magenta);

		//sync - save new mods to known
		_modsKnown = _modsKnown.Where(b => !_mods.Any(m => m.IsSameMod(b))).ToList();
		_modsKnown.AddRange(_mods);

		//add - all mods, and dependencies if not added
		foreach (var modInput in modsInput)
		{
			var mod = SyncMod(modInput);
			if (mod == null)
				continue;
			SyncDependencies(mod, modInput);
		}

		//delete - delete mods that were not found in .minecraft/mods but are added
		var names = modsInput.Select(m2 => m2.Name);
		var deleted = _mods.Where(m => !names.Contains(m.Name));
		foreach (var mod in deleted)
			ModExtensions.Log($"McMod {mod.Name} was deleted", ConsoleColor.Red);

		_mods = _mods.Except(deleted).ToList();

		ModExtensions.Log("Synchronization completed", ConsoleColor.Magenta);
	}
	private McMod? SyncMod(McMod modInput)
	{
		//skip - already added
		var mod = _mods.FirstOrDefault(m => m.HasExactName(modInput.Name));
		if (mod != null)
			return mod;

		//update - mod already added but with different version
		mod = _mods.FirstOrDefault(m => m.IsSameMod(modInput));
		if (mod != null)
		{
			ModExtensions.Log($"McMod {modInput.Name} was updated from {mod.Name}", ConsoleColor.Yellow);
			mod.Name = modInput.Name;
			return mod;
		}

		//ignore if not added and is a dependency
		mod = _mods.FirstOrDefault(m => m.Dependencies.ContainsName(modInput.Name));
		if (mod != null)
			return null;

		//move from known and update - mod is not added but is known
		mod = _modsKnown.FirstOrDefault(m => m.IsSameMod(modInput));
		if (mod != null)
		{
			ModExtensions.Log($"McMod {modInput.Name} was moved from known {mod.Name}", ConsoleColor.Green);
			mod.Name = modInput.Name;
			_mods.Add(mod);
			return mod;
		}

		//add
		ModExtensions.Log($"McMod {modInput.Name} was added", ConsoleColor.Green);
		_mods.Add(modInput);
		return modInput;
	}
	private static void SyncDependencies(McMod mod, McMod modInput)
	{
		var toAdd = modInput.Dependencies.Except(mod.Dependencies);
		foreach (var dependency in toAdd)
		{
			var oldDependency = mod.Dependencies.FirstOrDefault(d => d.SameName(dependency));
			if (oldDependency != null) //update dependency
			{
				ModExtensions.Log($"Dependency {dependency} was updated from {oldDependency} for mod {mod.Name}", ConsoleColor.DarkYellow);
				mod.Dependencies.Remove(oldDependency);
				mod.Dependencies.Add(dependency);
			}
			else //add dependency
			{
				ModExtensions.Log($"Dependency added {dependency} to {mod.Name}", ConsoleColor.DarkGreen);
				mod.Dependencies.Add(dependency);
			}
		}

		//remove dependency log
		var unused = mod.Dependencies.Except(modInput.Dependencies);
		foreach (var u in unused)
			ModExtensions.Log($"Dependency {u} added in {mod.Name} but not required", ConsoleColor.DarkRed);
	}
}
*/

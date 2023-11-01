namespace McHelper;

public class Mod
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public bool WorksConfirmed { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public Category Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = new List<string>();
    [JsonIgnore]
    public TomlTable? MetaData { get; set; }

    public bool HasExactName(string name)
    {
        return Name.Equals(name);
    }
    public bool HasName(string name)
    {
        return Name.SameName(name);
    }
    public bool IsSameMod(Mod mod)
    {
        return HasName(mod.Name) || (!string.IsNullOrEmpty(Id) && Id.Equals(mod.Id, StringComparison.InvariantCultureIgnoreCase));
    }
}

public enum Category
{
    Unknown = 0,
    Content = 1,
    Look = 2,
    Convention = 3,
    Difficulty = 4,
    Helper = 5,
    Dependency = 6,
    Client = 7,
}

public enum Priority
{
    Unknown = 0,
    Maybe = 1,
    Cool = 2,
    Necessary = 3,
}
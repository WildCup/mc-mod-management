using McHelper.Domain.Models.Enums;

namespace McHelper.Domain.Models;

public class Mod : EntityBase
{
	public required string Name { get; set; }
	public required string ForgeId { get; set; }
	public string Description { get; set; } = string.Empty;
	public Priority Priority { get; set; } = Priority.Unknown;
	public Category Category { get; set; } = Category.Unknown;
	public bool WorksConfirmed { get; set; } = false;
	public required string Path { get; set; }
	public List<int> Dependencies { get; set; } = [];
	public required int UserId { get; set; }
	public required User User { get; set; }
}


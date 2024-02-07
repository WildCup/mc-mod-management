namespace Application.Dtos.McMod;

using McHelper.Domain.Models;

public class UpdateMcModDto
{
	public required int Id { get; set; }
	public required string Name { get; set; }
	public required string ForgeId { get; set; }
	public required string Description { get; set; }
	public required Priority Priority { get; set; }
	public required Category Category { get; set; }
	public required bool WorksConfirmed { get; set; }
	public required List<int> Dependencies { get; set; } = [];
}
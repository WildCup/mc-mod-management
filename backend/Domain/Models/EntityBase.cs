using McHelper.Domain.Models.Enums;

namespace McHelper.Domain.Models;

public class EntityBase
{
	public required int Id { get; set; }
	public required DateTime ModifiedOn { get; set; } = DateTime.Now;
	public required DateTime CreatedOn { get; set; } = DateTime.Now;
}


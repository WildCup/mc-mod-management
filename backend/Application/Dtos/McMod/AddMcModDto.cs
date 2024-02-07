namespace Application.Dtos.McMod;

using Microsoft.AspNetCore.Http;

public class AddMcModDto
{
	public required IFormFile File { get; set; }
}

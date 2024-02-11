namespace Application.Dtos.Mod;

using Microsoft.AspNetCore.Http;

public class AddModDto
{
	public required IFormFile File { get; set; }
}

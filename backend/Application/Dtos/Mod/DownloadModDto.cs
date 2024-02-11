namespace Application.Dtos.Mod;

using Microsoft.AspNetCore.Http;

public class DownloadModDto
{
	public required IFormFile File { get; set; }
	public required string Url { get; set; }
}

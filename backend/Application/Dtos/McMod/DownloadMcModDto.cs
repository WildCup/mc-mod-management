namespace Application.Dtos.McMod;

using Microsoft.AspNetCore.Http;

public class DownloadMcModDto
{
	public required IFormFile File { get; set; }
	public required string Url { get; set; }
}

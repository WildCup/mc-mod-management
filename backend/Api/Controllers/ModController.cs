namespace Api.Controllers;

using Application.Dtos.Mod;
using McHelper.Application.Services;
using McHelper.Domain.Models;
// using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// [Authorize]
[ApiController]
[Route("mod")]
public class ModController(IModService service) : ControllerBase
{
	private readonly List<Mod> _mods = [];
	private readonly IModService _service = service;

	[HttpGet]
	public ActionResult<IEnumerable<GetModDto>> GetAll()
	{
		return Ok(_service.GetAll());
	}

	[HttpPost]
	public ActionResult<Mod> Add(AddModDto mod)
	{
		return Ok(_mods);
	}

	[HttpPut("{id:int}")]
	public ActionResult<Mod> Update(int id, UpdateModDto mod)
	{
		return Ok(_mods);
	}
}

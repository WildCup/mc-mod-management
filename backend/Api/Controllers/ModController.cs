namespace Api.Controllers;

using Application.Dtos.McMod;
using McHelper.Domain.Models;
// using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// [Authorize]
[ApiController]
[Route("mod")]
public class ModController : ControllerBase
{
	private readonly List<McMod> _mods = [];
	public ModController()
	{

	}

	[HttpGet]
	public ActionResult<McMod> GetAll()
	{
		return Ok(_mods);
	}

	[HttpPost]
	public ActionResult<McMod> Add(AddMcModDto mod)
	{
		return Ok(_mods);
	}

	[HttpPut("{id:int}")]
	public ActionResult<McMod> Update(int id, UpdateMcModDto mod)
	{
		return Ok(_mods);
	}
}

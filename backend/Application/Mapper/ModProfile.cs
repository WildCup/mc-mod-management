namespace McHelper.Application.Logic;

using AutoMapper;
using global::Application.Dtos.McMod;
using McHelper.Domain.Models;

public class ModProfile : Profile
{
	public ModProfile()
	{
		_ = CreateMap<McMod, GetMcModDto>();
	}
}

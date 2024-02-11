namespace McHelper.Application.Logic;

using AutoMapper;
using global::Application.Dtos.Mod;
using McHelper.Domain.Models;

public class ModProfile : Profile
{
	public ModProfile()
	{
		_ = CreateMap<Mod, GetModDto>();
	}
}

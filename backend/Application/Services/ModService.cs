using Application.Dtos.Mod;
using AutoMapper;
using McHelper.Domain.Abstractions;

namespace McHelper.Application.Services;

public class ModService(IModRepository repository, IMapper mapper) : ServiceBase(mapper), IModService
{
	private readonly IModRepository _repository = repository;

	public IEnumerable<GetModDto> GetAll()
	{
		var mods = _repository.GetAll();
		return _mapper.Map<IEnumerable<GetModDto>>(mods)!;
	}
}

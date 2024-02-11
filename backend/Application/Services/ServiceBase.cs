using AutoMapper;

namespace McHelper.Application.Services;

public abstract class ServiceBase(IMapper mapper)
{
	protected readonly IMapper _mapper = mapper;
}

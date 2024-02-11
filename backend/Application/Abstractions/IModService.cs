using Application.Dtos.Mod;

namespace McHelper.Application.Services;

public interface IModService : IServiceBase
{
	IEnumerable<GetModDto> GetAll();
}

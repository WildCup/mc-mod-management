using Infrastructure.Data;
using McHelper.Domain.Abstractions;
using McHelper.Domain.Models;

namespace Infrastructure.Repositories;

public class ModRepository(DataContext dataContext) : RepositoryBase<Mod>(dataContext), IModRepository
{
}

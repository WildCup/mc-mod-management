using Infrastructure.Data;
using McHelper.Domain.Abstractions;
using McHelper.Domain.Models;

namespace Infrastructure.Repositories;

public class UserRepository(DataContext dataContext) : RepositoryBase<User>(dataContext), IUserRepository
{
}

using McHelper.Domain.Models;

namespace McHelper.Domain.Abstractions;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
	Task<TEntity> Add(TEntity entity);
	Task<TEntity> Update(TEntity entity);
	Task Delete(TEntity entity);
	Task<IEnumerable<TEntity>> GetAll();
	Task<TEntity> GetById(int id);
	Task<TEntity?> FindById(int id);
}

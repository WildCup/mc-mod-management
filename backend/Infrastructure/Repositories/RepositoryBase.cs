using Infrastructure.Data;
using McHelper.Domain.Abstractions;
using McHelper.Domain.Models;
using McHelper.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity>(DataContext dataContext) : IRepositoryBase<TEntity> where TEntity : EntityBase
{
	private readonly DataContext _dataContext = dataContext;

	public async Task<TEntity> Add(TEntity entity)
	{
		var added = await _dataContext.Set<TEntity>().AddAsync(entity);
		await _dataContext.SaveChangesAsync();
		return added.Entity;
	}

	public async Task<TEntity> Update(TEntity entity)
	{
		await _dataContext.SaveChangesAsync();
		return entity;
	}

	public async Task Delete(TEntity entity)
	{
		_dataContext.Set<TEntity>().Remove(entity);
		await _dataContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<TEntity>> GetAll()
	{
		return await _dataContext.Set<TEntity>().ToListAsync();
	}

	public async Task<TEntity> GetById(int id)
	{
		return await FindById(id) ?? throw new EntityNotFoundException(typeof(TEntity), id);
	}

	public async Task<TEntity?> FindById(int id)
	{
		return await _dataContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
	}
}

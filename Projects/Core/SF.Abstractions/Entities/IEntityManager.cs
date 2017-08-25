using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;

namespace SF.Entities
{
	[Flags]
	public enum EntityManagerCapability
	{
		Creatable=1,
		Updatable=2,
		Deletable=4,
		All=7
	}
	public interface IEntityRemover<TKey>
	{
		Task RemoveAsync(TKey Key);
		Task RemoveAllAsync();
	}
	public interface IEntityManager<TKey>: IEntityRemover<TKey>
	{
		EntityManagerCapability Capabilities { get; }
	}
	public interface IEntityEditableLoader<TKey, TEntity>
		where TKey : IEquatable<TKey>
		where TEntity : class, IEntityWithId<TKey>
	{
		Task<TEntity> LoadForEdit(TKey Id);
	}
	public interface IEntityUpdator<TKey, TEntity>
		where TKey : IEquatable<TKey>
		where TEntity : class, IEntityWithId<TKey>
	{
		Task UpdateAsync(TEntity Entity);
	}
	public interface IEntityCreator<TKey, TEntity>
		where TKey : IEquatable<TKey>
		where TEntity : class, IEntityWithId<TKey>
	{
		Task<TKey> CreateAsync(TEntity Entity);
	}
	public interface IEntityManager<TKey, TEntity>:
		IEntityManager<TKey>,
		IEntityEditableLoader<TKey,TEntity>,
		IEntityCreator<TKey,TEntity>,
		IEntityUpdator<TKey, TEntity>
		where TKey:IEquatable<TKey>
		where TEntity:class,IEntityWithId<TKey>
	{
		
		
	}
}

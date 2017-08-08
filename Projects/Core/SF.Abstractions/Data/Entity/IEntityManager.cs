using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;

namespace SF.Data.Entity
{
	[Flags]
	public enum EntityManagerCapability
	{
		Creatable=1,
		Updatable=2,
		Deletable=4,
		All=7
	}
	public interface IEntityManager<TKey>
	{
		EntityManagerCapability Capabilities { get; }
		Task RemoveAsync(TKey Key);
		Task RemoveAllAsync();
	}
	public interface IEntityManager<TKey, TEntity>:
		IEntityManager<TKey>
		where TKey:IEquatable<TKey>
		where TEntity:class,IObjectWithId<TKey>
	{
		Task<TEntity> LoadForEdit(TKey Id);
		Task<TKey> CreateAsync(TEntity Entity);
		Task UpdateAsync(TEntity Entity);
	}
}

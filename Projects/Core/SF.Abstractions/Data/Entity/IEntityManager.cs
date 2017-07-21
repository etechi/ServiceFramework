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
	public interface IEntityManager<TKey, TEntity>
		where TKey:IEquatable<TKey>
		where TEntity:class,IObjectWithId<TKey>
	{
		EntityManagerCapability Capabilities { get; }
		Task<TEntity> LoadForEdit(TKey Id);
		Task<TKey> CreateAsync(TEntity Entity);
		Task UpdateAsync(TEntity Entity);
		Task RemoveAsync(TKey Key);
	}
}

using System.Threading.Tasks;
using System;
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
	{
		EntityManagerCapability Capabilities { get; }
		Task<TEntity> LoadForEdit(TKey Id);
		Task<TKey> CreateAsync(TEntity Entity);
		Task UpdateAsync(TEntity Entity);
		Task DeleteAsync(TKey Key);
	}
}

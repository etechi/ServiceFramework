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
	public interface IEntityRemover<TEditable>
	{
		Task<TEditable> RemoveAsync(TEditable Key);
	}
	public interface IEntityAllRemover
	{
		Task RemoveAllAsync();
	}
	public interface IEntityManagerCapabilities 
	{
		EntityManagerCapability Capabilities { get; }
	}
	public interface IEntityEditableLoader<TEntity>
		where TEntity : class
	{
		Task<TEntity> LoadForEdit(TEntity Key);
	}
	public interface IEntityUpdator<TEntity>
		where TEntity : class
	{
		Task<TEntity> UpdateAsync(TEntity Entity);
	}
	public interface IEntityCreator<TEntity>
		where TEntity : class
	{
		Task<TEntity> CreateAsync(TEntity Entity);
	}
	public interface IEntityManager<TEntity>:
		IEntityManagerCapabilities,
		IEntityRemover<TEntity>,
		IEntityAllRemover,
		IEntityEditableLoader<TEntity>,
		IEntityCreator<TEntity>,
		IEntityUpdator<TEntity>
		where TEntity:class
	{
		
		
	}
}

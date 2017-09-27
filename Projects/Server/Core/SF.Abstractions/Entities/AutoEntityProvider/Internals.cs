using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider
{
	public interface IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		   where TEntityDetail : class, IEntityWithId<TKey>
		   where TEntitySummary : class, IEntityWithId<TKey>
		   where TEntityEditable : class, IEntityWithId<TKey>
		   where TKey : IEquatable<TKey>
		   where TQueryArgument : IQueryArgument<TKey>
	{
		EntityManagerCapability Capabilities { get; }
		IDataSetEntityManager EntityManager { get; }
		Task<TEntityDetail> GetAsync(TKey Id);
		Task<TEntityDetail[]> GetAsync(TKey[] Ids);
		Task<TKey> CreateAsync(TEntityEditable Entity);
		Task<TEntityEditable> LoadForEdit(TKey Id);
		Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging);
		Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging);
		Task RemoveAllAsync();
		Task RemoveAsync(TKey Key);
		Task UpdateAsync(TEntityEditable Entity);
	}

	public interface IValueTypeResolver
	{
		IValueType Resolve(string TypeName,string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes);
	}
}

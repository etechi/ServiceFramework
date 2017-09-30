using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider
{
	public interface IValueConverter
	{
		Type TempFieldType { get; }
		Expression SourceToTemp(Expression src, PropertyInfo srcProp);
	}
	public interface IValueConverter<T, D> : IValueConverter
	{
		Task<D> TempToDest(object src, T value);
	}
	public interface IEntityPropertyModifier
	{ }
	public interface IEntityModifierValueConverter<T, D> : IEntityPropertyModifier
	{
		Task<D> Convert(object src, T value,bool existValue);
	}

	
	public interface IEntityModifier<TEntity,TDataModel> 
		where TDataModel:class
	{
		Func<IDataSetEntityManager<TEntity,TDataModel>,IEntityModifyContext<TEntity,TDataModel>,Task> Modifier { get; }
	}
	public interface IDataSetAutoEntityProvider<TKey,TDetail, TSummary, TEditable, TQueryArgument>
	{
		EntityManagerCapability Capabilities { get; }
		IDataSetEntityManager EntityManager { get; }
		Task<TDetail> GetAsync(TKey Id);
		Task<TDetail[]> GetAsync(TKey[] Ids);
		Task<TKey> CreateAsync(TEditable Entity);
		Task<TEditable> LoadForEdit(TKey Id);
		Task<QueryResult<TSummary>> QueryAsync(TQueryArgument Arg, Paging paging);
		Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging);
		Task RemoveAllAsync();
		Task RemoveAsync(TKey Key);
		Task UpdateAsync(TEditable Entity);
	}

	public interface IValueTypeResolver
	{
		IValueType Resolve(string TypeName,string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes);
	}
}

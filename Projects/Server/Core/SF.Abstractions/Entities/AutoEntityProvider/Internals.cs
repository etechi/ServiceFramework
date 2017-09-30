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
	{
		int Priority { get; }
	}
	public interface IAsyncEntityPropertyModifier<TEntityPropertyValue, TDataModelPropertyValue> :
		IEntityPropertyModifier
	{
		Task<TDataModelPropertyValue> Execute(
			IDataSetEntityManager Manager, 
			IEntityModifyContext Context, 
			TEntityPropertyValue Value
			);
	}

	public interface IAsyncEntityPropertyModifier<TDataModelPropertyValue> :
		IEntityPropertyModifier
	{
		Task<TDataModelPropertyValue> Execute(
			IDataSetEntityManager Manager,
			IEntityModifyContext Context
			);
	}
	public interface IEntityPropertyModifier<TDataModelPropertyValue> :
		IEntityPropertyModifier
	{
		TDataModelPropertyValue Execute(
			IDataSetEntityManager Manager,
			IEntityModifyContext Context
			);
	}
	public interface IEntityPropertyModifier<TEntityPropertyValue, TDataModelPropertyValue> :
		IEntityPropertyModifier
	{
		TDataModelPropertyValue Execute(
			IDataSetEntityManager Manager,
			IEntityModifyContext Context,
			TEntityPropertyValue Value
			);
	}


	public interface IEntityPropertyModifierProvider
	{
		IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType,
			Type EntityType,
			PropertyInfo EntityProperty,
			Type DataModelType,
			PropertyInfo DataModelProperty
			);
			
	}

	//public interface IEntityModifier
	public interface IEntityModifier {
		int Priority { get; }
	}
	public interface IEntityModifier<TEntity,TDataModel> : IEntityModifier
		where TDataModel:class
	{
		Task Execute(IDataSetEntityManager<TEntity, TDataModel> Manager, IEntityModifyContext<TEntity, TDataModel> Context);
	}
	public interface IEntityModifierProvider
	{
		IEntityModifier<TEntity, TDataModel> GetEntityModifier<TEntity, TDataModel>(DataActionType ActionType)
			where TDataModel : class;
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

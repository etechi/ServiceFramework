#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider
{
	public interface IEntityPropertyQueryConverter
	{
		Type TempFieldType { get; }
		Expression SourceToTemp(Expression src, PropertyInfo srcProp);
	}
	public interface IEntityPropertyQueryConverter<TTempType, TEntityPropType> : IEntityPropertyQueryConverter
	{
		Task<TEntityPropType> TempToDest(object src, TTempType value);
	}
	public interface IEntityPropertyQueryConverterProvider
	{
		int Priority { get; }
		IEntityPropertyQueryConverter GetPropertyConverter(
			PropertyInfo DataModelProperty,
			PropertyInfo EntityProperty
			);
	}
	public interface IEntityPropertyModifier
	{
		int Priority { get; }
		IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier);
	}
	public interface INoneEntityPropertyModifier : IEntityPropertyModifier
	{

	}
	public interface IAsyncEntityPropertyModifier<TEntityPropertyValue, TDataModelPropertyValue> :
		IEntityPropertyModifier
	{
		Task<TDataModelPropertyValue> Execute(
			IDataSetEntityManager Manager, 
			IEntityModifyContext Context,
			TDataModelPropertyValue OrgValue,
			TEntityPropertyValue NewValue
			);
	}

	public interface IAsyncEntityPropertyModifier<TDataModelPropertyValue> :
		IEntityPropertyModifier
	{
		Task<TDataModelPropertyValue> Execute(
			IDataSetEntityManager Manager,
			IEntityModifyContext Context,
			TDataModelPropertyValue OrgValue
			);
	}
	public interface IEntityPropertyModifier<TDataModelPropertyValue> :
		IEntityPropertyModifier
	{
		TDataModelPropertyValue Execute(
			IDataSetEntityManager Manager,
			IEntityModifyContext Context,
			TDataModelPropertyValue OrgValue
			);
	}
	public interface IEntityPropertyModifier<TEntityPropertyValue, TDataModelPropertyValue> :
		IEntityPropertyModifier
	{
		TDataModelPropertyValue Execute(
			IDataSetEntityManager Manager,
			IEntityModifyContext Context,
			TDataModelPropertyValue OrgValue,
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

	public interface IEntityModifierCache : IEntityModifierProvider
	{
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

	public interface IDataSetAutoEntityProviderCache
	{
		IDataSetAutoEntityProvider<TKey, TDetail, TSummary, TEditable, TQueryArgument> Create<TKey, TDetail, TSummary, TEditable, TQueryArgument>(IServiceProvider sp);
		IDataSetAutoEntityProvider<TKey, TDetail, TSummary, TEditable, TQueryArgument> Create<TKey, TDetail, TSummary, TEditable, TQueryArgument,TDataModel>(IServiceProvider sp) where TDataModel : class;
	}

	public interface IDataSetAutoEntityProviderFactory
	{
		IDataSetAutoEntityProvider<TKey, TDetail, TSummary, TEditable, TQueryArgument> Create<TKey, TDetail, TSummary, TEditable, TQueryArgument>();
		IDataSetAutoEntityProvider<TKey, TDetail, TSummary, TEditable, TQueryArgument> Create<TKey, TDetail, TSummary, TEditable, TQueryArgument, TDataModel>() where TDataModel:class;
	}

	public interface IValueTypeProvider
	{
		int Priority { get; }
		IValueType DetectValueType(string TypeName, string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes);
	}
	public interface IValueTypeResolver
	{
		IValueType Resolve(string TypeName,string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes);
	}
}

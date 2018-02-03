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

using SF.Sys.Services;
using System;
using System.Threading.Tasks;

namespace SF.Sys.Entities.AutoEntityProvider
{
	public interface IDataSetAutoEntityProviderSetting
	{
		Lazy<Func<IServiceProvider, object>> FuncCreateProvider { get; }
	}
	public interface IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		: IDataSetAutoEntityProviderSetting
		where TQueryArgument:IPagingArgument
	{
		Task<TKey> CreateAsync(IEntityServiceContext ServiceContext, TEntityEditable Entity);
		Task<TEntityDetail> GetAsync(IEntityServiceContext ServiceContext, TKey Id,int DetailModelDeep, string[] Properties);

		Task<TEntityDetail[]> GetAsync(IEntityServiceContext ServiceContext, TKey[] Ids,string[] Properties, int DetailModelDeep);

		Task<TEntityEditable> LoadForEdit(IEntityServiceContext ServiceContext, TKey Id, int EditableModelDeep);

		Task<QueryResult<TEntitySummary>> QueryAsync(IEntityServiceContext ServiceContext, TQueryArgument Arg);

		Task<QueryResult<TKey>> QueryIdentsAsync(IEntityServiceContext ServiceContext, TQueryArgument Arg);

		Task RemoveAllAsync(IEntityServiceContext ServiceContext);

		Task RemoveAsync(IEntityServiceContext ServiceContext, TKey Key);

		Task UpdateAsync(IEntityServiceContext ServiceContext, TEntityEditable Entity);
	}
	class DataSetAutoEntityProviderSetting<TKey,TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument,TDataModel>
		: IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		where TDataModel : class,new()
		where TQueryArgument:IPagingArgument
	{

		public Lazy<Func<IServiceProvider, object>> FuncCreateProvider { get; }


		public DataSetAutoEntityProviderSetting()
		{
			FuncCreateProvider = new Lazy<Func<IServiceProvider, object>>(() =>
				new Func<IServiceProvider, object>(
					sp => new DataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(
							sp.Resolve<IEntityServiceContext>(),
							this
						)
				)
			 );

		}
		public virtual Task<TKey> CreateAsync(IEntityServiceContext ServiceContext, TEntityEditable Entity)
		{
			return ServiceContext.CreateAsync<TKey, TEntityEditable, TDataModel>(
				Entity,
				null,
				null,
				null,
				false
				);
		}

		public Task<TEntityDetail> GetAsync(IEntityServiceContext ServiceContext, TKey Id,int DetailModelDeep, string[] Properties)
		{
			return ServiceContext.AutoGetAsync<TKey,TEntityDetail,TDataModel>(Id,DetailModelDeep, Properties);
		}

		public Task<TEntityDetail[]> GetAsync(IEntityServiceContext ServiceContext, TKey[] Ids,string[] Properties,int DetailModelDeep)
		{
			return ServiceContext.AutoBatchGetAsync<TKey,TEntityDetail,TDataModel>(Ids,Properties, DetailModelDeep);
		}

		public Task<TEntityEditable> LoadForEdit(IEntityServiceContext ServiceContext, TKey Id,int EditableModelDeep)
		{
			return ServiceContext.AutoLoadForEdit<TKey, TEntityEditable, TDataModel>(Id,EditableModelDeep);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(IEntityServiceContext ServiceContext, TQueryArgument Arg)
		{
			return ServiceContext.AutoQueryAsync<TEntitySummary, TQueryArgument, TDataModel>(Arg);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(IEntityServiceContext ServiceContext, TQueryArgument Arg)
		{
			return ServiceContext.AutoQueryIdentsAsync<TKey, TQueryArgument, TDataModel>(Arg);

		}

		public async Task RemoveAllAsync(IEntityServiceContext ServiceContext)
		{
			await ServiceContext.RemoveAllAsync<TKey,TEntityEditable, TDataModel>(
				async id =>
					await RemoveAsync(ServiceContext, id)
				);
		}

		public async Task RemoveAsync(IEntityServiceContext ServiceContext, TKey Key)
		{
			await ServiceContext.RemoveAsync<TKey, TEntityEditable, TDataModel>(
				Key,
				null,
				null
				);
		}

		public async Task UpdateAsync(IEntityServiceContext ServiceContext, TEntityEditable Entity)
		{
			await ServiceContext.UpdateAsync<TKey,TEntityEditable, TDataModel>(
				Entity,
				null,
				null
				);
		}

	}


}

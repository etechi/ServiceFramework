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
using System.Reflection;
using System.Linq.Expressions;
using SF.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using SF.Core.ServiceManagement;

namespace SF.Entities.AutoEntityProvider
{
	public interface IDataSetAutoEntityProviderSetting
	{
		Lazy<Func<IServiceProvider, object>> FuncCreateProvider { get; }
	}
	public interface IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		: IDataSetAutoEntityProviderSetting
	{
		Task<TKey> CreateAsync(IDataSetEntityManager EntityManager, TEntityEditable Entity);
		Task<TEntityDetail> GetAsync(IDataSetEntityManager EntityManager, TKey Id);

		Task<TEntityDetail[]> GetAsync(IDataSetEntityManager EntityManager, TKey[] Ids);

		Task<TEntityEditable> LoadForEdit(IDataSetEntityManager EntityManager, TKey Id);

		Task<QueryResult<TEntitySummary>> QueryAsync(IDataSetEntityManager EntityManager, TQueryArgument Arg, Paging paging);

		Task<QueryResult<TKey>> QueryIdentsAsync(IDataSetEntityManager EntityManager, TQueryArgument Arg, Paging paging);

		Task RemoveAllAsync(IDataSetEntityManager EntityManager);

		Task RemoveAsync(IDataSetEntityManager EntityManager, TKey Key);

		Task UpdateAsync(IDataSetEntityManager EntityManager, TEntityEditable Entity);
	}
	class DataSetAutoEntityProviderSetting<TKey,TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument,TDataModel>
		: IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		where TDataModel : class,new()
	{

		public Lazy<Func<IServiceProvider, object>> FuncCreateProvider { get; }


		public DataSetAutoEntityProviderSetting()
		{
			FuncCreateProvider = new Lazy<Func<IServiceProvider, object>>(() =>
				new Func<IServiceProvider, object>(
					sp => new DataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(
							sp.Resolve<IDataSetEntityManager<TEntityEditable, TDataModel>>(),
							this
						)
				)
			 );

		}
		public Task<TKey> CreateAsync(IDataSetEntityManager EntityManager,TEntityEditable Entity)
		{
			var em = (IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager;
			return em.CreateAsync<TKey, TEntityEditable, TDataModel>(
				Entity,
				null,
				null,
				null,
				true
				);
		}

		public Task<TEntityDetail> GetAsync(IDataSetEntityManager EntityManager, TKey Id)
		{
			var em = (IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager;
			return em.AutoGetAsync<TKey,TEntityDetail,TDataModel>(Id);
		}

		public Task<TEntityDetail[]> GetAsync(IDataSetEntityManager EntityManager, TKey[] Ids)
		{
			var em = (IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager;
			return em.AutoBatchGetAsync<TKey,TEntityDetail,TDataModel>(Ids);
		}

		public Task<TEntityEditable> LoadForEdit(IDataSetEntityManager EntityManager, TKey Id)
		{
			var em = ((IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager);
			return em.AutoLoadForEdit<TKey, TEntityEditable, TDataModel>(Id);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(IDataSetEntityManager EntityManager, TQueryArgument Arg, Paging paging)
		{
			var em = ((IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager);
			return em.AutoQueryAsync<TEntitySummary, TQueryArgument, TDataModel>(Arg, paging);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(IDataSetEntityManager EntityManager, TQueryArgument Arg, Paging paging)
		{
			var em = ((IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager);
			return em.AutoQueryIdentsAsync<TKey, TQueryArgument, TDataModel>(Arg, paging);

		}

		public async Task RemoveAllAsync(IDataSetEntityManager EntityManager)
		{
			var em = ((IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager);
			await em.RemoveAllAsync<TKey,TEntityEditable, TDataModel>(
				async id =>
					await RemoveAsync(em,id)
				);
		}

		public async Task RemoveAsync(IDataSetEntityManager EntityManager, TKey Key)
		{
			var em = ((IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager);
			await em.RemoveAsync<TKey, TEntityEditable, TDataModel>(
				Key,
				null,
				null,
				true
				);
		}

		public async Task UpdateAsync(IDataSetEntityManager EntityManager, TEntityEditable Entity)
		{
			var em = ((IDataSetEntityManager<TEntityEditable,TDataModel>)EntityManager);
			await em.UpdateAsync<TKey,TEntityEditable, TDataModel>(
				Entity,
				null,
				null,
				true
				);
		}

	}


}

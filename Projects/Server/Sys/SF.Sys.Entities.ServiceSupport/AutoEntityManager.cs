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
using System.Threading.Tasks;

namespace SF.Sys.Entities.AutoEntityProvider
{
	public class AutoEntityManager<TKey, TDetail, TSummary, TEditable, TQueryArgument,TDataModel> :
		AutoEntityManager<TKey, TDetail, TSummary, TEditable, TQueryArgument>
		where TDataModel:class
		where TEditable : class
		where TQueryArgument:IPagingArgument
	{
		public AutoEntityManager(
			IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory
			) :base(DataSetAutoEntityProviderFactory)
		{
		}
		protected override IDataSetAutoEntityProvider<TKey, TDetail, TSummary, TEditable, TQueryArgument> OnCreateAutoEntityProvider(
			IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory
			)
		{
			return DataSetAutoEntityProviderFactory.Create<TKey, TDetail, TSummary, TEditable, TQueryArgument,TDataModel>();
		}
	}

	public class AutoEntityManager<TKey,TDetail, TSummary,TEditable, TQueryArgument> :
		IEntityLoadable<TKey, TDetail>,
		IEntityBatchLoadable<TKey, TDetail>,
		IEntityQueryable<TSummary, TQueryArgument>,
		IEntityManagerCapabilities,
		IEntityEditableLoader<TKey, TEditable>,
		IEntityCreator<TKey, TEditable>,
		IEntityUpdator<TEditable>,
		IEntityRemover<TKey>,
		IEntityAllRemover
		where TEditable : class
		where TQueryArgument:IPagingArgument
	{
		protected IDataSetAutoEntityProvider<TKey,TDetail, TSummary, TEditable, TQueryArgument> AutoEntityProvider { get; }
		protected IEntityServiceContext ServiceContext => AutoEntityProvider.ServiceContext;
		public AutoEntityManager(IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory)
		{
			this.AutoEntityProvider = OnCreateAutoEntityProvider(DataSetAutoEntityProviderFactory);
		}
		protected virtual IDataSetAutoEntityProvider<TKey, TDetail, TSummary, TEditable, TQueryArgument> OnCreateAutoEntityProvider(IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory)
		{
			return DataSetAutoEntityProviderFactory.Create<TKey, TDetail, TSummary, TEditable, TQueryArgument>();
		}
		public EntityManagerCapability Capabilities => AutoEntityProvider.Capabilities;

		public long? ServiceInstanceId => ServiceContext.ServiceInstanceDescroptor.InstanceId;

		public Task<TKey> CreateAsync(TEditable Entity)
		{
			return AutoEntityProvider.CreateAsync( Entity);
		}

		public Task<TDetail> GetAsync(TKey Id, string[] Fields = null)
		{
			return AutoEntityProvider.GetAsync(Id,Fields);
		}

		public Task<TDetail[]> BatchGetAsync(TKey[] Ids,string[] Properties)
		{
			return AutoEntityProvider.GetAsync(Ids,Properties);
		}

		public Task<TEditable> LoadForEdit(TKey Id)
		{
			return AutoEntityProvider.LoadForEdit(Id);
		}

		public Task<QueryResult<TSummary>> QueryAsync(TQueryArgument Arg)
		{
			return AutoEntityProvider.QueryAsync(Arg);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg)
		{
			return AutoEntityProvider.QueryIdentsAsync(Arg);
		}

		public Task RemoveAllAsync()
		{
			return AutoEntityProvider.RemoveAllAsync();
		}

		public Task RemoveAsync(TKey Key)
		{
			return AutoEntityProvider.RemoveAsync(Key);
		}

		public Task UpdateAsync(TEditable Entity)
		{
			return AutoEntityProvider.UpdateAsync(Entity);
		}
	}


}

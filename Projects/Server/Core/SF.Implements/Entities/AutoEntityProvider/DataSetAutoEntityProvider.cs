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
using SF.Core.ServiceManagement;

namespace SF.Entities.AutoEntityProvider
{
	class DataSetAutoEntityProvider<TKey,TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>:
		IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
	{
	

		public IEntityServiceContext ServiceContext { get; }
		IEntityServiceContext IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>.ServiceContext => ServiceContext;
		IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> Setting { get; }

		public DataSetAutoEntityProvider(IEntityServiceContext ServiceContext, IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> Setting)
		{
			this.ServiceContext = ServiceContext;
			this.Setting = Setting;

		}
		public EntityManagerCapability Capabilities { get; } = EntityManagerCapability.All;


		public Task<TKey> CreateAsync( TEntityEditable Entity)
		{
			return Setting.CreateAsync(ServiceContext, Entity);
		}

		public Task<TEntityDetail> GetAsync(TKey Id)
		{
			return Setting.GetAsync(ServiceContext, Id);
		}

		public Task<TEntityDetail[]> GetAsync(TKey[] Ids)
		{
			return Setting.GetAsync(ServiceContext, Ids);
		}

		public Task<TEntityEditable> LoadForEdit(TKey Id)
		{
			return Setting.LoadForEdit(ServiceContext, Id);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return Setting.QueryAsync(ServiceContext, Arg, paging);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return Setting.QueryIdentsAsync(ServiceContext, Arg, paging); 
		}

		public Task RemoveAllAsync()
		{
			return Setting.RemoveAllAsync(ServiceContext);
		}

		public Task RemoveAsync(TKey Key)
		{
			return Setting.RemoveAsync(ServiceContext, Key);
		}

		public Task UpdateAsync(TEntityEditable Entity)
		{
			return Setting.UpdateAsync(ServiceContext, Entity);
		}
	}


}

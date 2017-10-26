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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SF.Data;
using SF.Core.Times;
using SF.Core.Logging;
using SF.Core.ServiceManagement;
using System.Collections.Generic;
using SF.ADT;

namespace SF.Entities
{
	public abstract class BaseEntityManager: IManagedServiceWithId
	{
		protected IEntityManager EntityManager { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor => EntityManager.ServiceInstanceDescroptor;
		public DateTime Now => EntityManager.Now;
		public ILogger Logger => EntityManager.Logger;
		public IIdentGenerator IdentGenerator => EntityManager.IdentGenerator;
		
		public long? ServiceInstanceId => EntityManager.ServiceInstanceDescroptor.InstanceId;
		public long? DataScopeId => EntityManager.ServiceInstanceDescroptor.DataScopeId;

		public BaseEntityManager(IEntityManager EntityManager)
		{
			this.EntityManager = EntityManager;
		}
	}

	public abstract class BaseDataSetEntityManager<TModel>:
		BaseEntityManager
		where TModel:class
	{
		protected new IReadOnlyDataSetEntityManager<TModel> EntityManager => (IReadOnlyDataSetEntityManager<TModel>)base.EntityManager;
		public IDataSet<TModel> DataSet => EntityManager.DataSet;
		public IDataContext DataContext => DataSet.Context;
		public new IIdentGenerator<TModel> IdentGenerator => EntityManager.IdentGenerator;

		public BaseDataSetEntityManager(IReadOnlyDataSetEntityManager<TModel> EntityManager):base(EntityManager)
		{
		}
	}

	public abstract class EntitySource<TKey, TEntityDetail, TModel> :
		EntitySource<TKey, TEntityDetail, TEntityDetail, TModel>
		where TModel : class
	{
		public EntitySource(IReadOnlyDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override async Task<TEntityDetail[]> OnPrepareDetails(TEntityDetail[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
			return Internals;
		}
	}
	
	public abstract class EntitySource<TKey,TDetail, TDetailTemp, TModel> :
		BaseDataSetEntityManager<TModel>,
		IEntityLoadable<TKey, TDetail>,
		IEntityBatchLoadable<TKey, TDetail>
		where TModel: class
	{
		public EntitySource(IReadOnlyDataSetEntityManager<TModel> EntityManager):base(EntityManager)
		{
		}
		protected virtual IContextQueryable<TDetailTemp> OnMapModelToDetail(IContextQueryable<TModel> Query)
		{
			return Query.Select(Poco.MapExpression<TModel, TDetailTemp>());
		}
		protected abstract Task<TDetail[]> OnPrepareDetails(TDetailTemp[] Internals);

		public Task<TDetail[]> GetAsync(TKey[] Ids)
		{
			return EntityManager.BatchGetAsync<TKey, TDetailTemp,TDetail,TModel>(Ids, OnMapModelToDetail,OnPrepareDetails);
		}

		public Task<TDetail> GetAsync(TKey Id)
		{
			return EntityManager.GetAsync<TKey, TDetailTemp, TDetail, TModel>(Id, OnMapModelToDetail, OnPrepareDetails);
		}
	}
	public abstract class AutoEntitySource<TKey, TDetail, TModel> :
	   BaseDataSetEntityManager<TModel>,
	   IEntityLoadable<TKey, TDetail>,
	   IEntityBatchLoadable<TKey, TDetail>
	   where TModel : class
	{
		public AutoEntitySource(IReadOnlyDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}
		public virtual Task<TDetail[]> GetAsync(TKey[] Ids)
		{
			return EntityManager.AutoBatchGetAsync<TKey, TDetail, TModel>(Ids);
		}

		public virtual Task<TDetail> GetAsync(TKey Id)
		{
			return EntityManager.AutoGetAsync<TKey, TDetail, TModel>(Id);
		}
	}
	public abstract class ConstantEntitySource<TKey, TEntityDetail> :
		ConstantEntitySource<TKey, TEntityDetail, TEntityDetail>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{
		public ConstantEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TEntityDetail> Models) : base(EntityManager, Models)
		{
		}

		protected override IContextQueryable<TEntityDetail> OnMapModelToInternal(IContextQueryable<TEntityDetail> Query)
		{
			return Query;
		}
	}
	public abstract class ConstantEntitySource<TKey, TEntityDetail, TModel> :
	   ConstantEntitySource<TKey, TEntityDetail, TEntityDetail, TModel>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TModel : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{
		public ConstantEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TModel> Models) : base(EntityManager, Models)
		{
		}

		protected override Task<TEntityDetail[]> OnPrepareInternals(TEntityDetail[] Internals)
		{
			return Task.FromResult(Internals);
		}
	}
	public abstract class ConstantEntitySource<TKey,TEntityDetail,TTemp,TModel> :
		BaseEntityManager,
		IEntityLoadable<TKey, TEntityDetail>,
		IEntityBatchLoadable<TKey, TEntityDetail>
		where TModel:class
	{

		protected IReadOnlyDictionary<TKey, TModel> Models { get; }
		public ConstantEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TModel> Models) :base(EntityManager)
		{
			this.Models = Models;
		}
		protected virtual IContextQueryable<TTemp> OnMapModelToInternal(IContextQueryable<TModel> Query)
		{
			return Query.Select(ADT.Poco.MapExpression<TModel, TTemp>());
		}
		protected abstract Task<TEntityDetail[]> OnPrepareInternals(TTemp[] Internals);
		public async Task<TEntityDetail> GetAsync(TKey Id)
		{
			if (Models.TryGetValue(Id, out var m))
			{
				var re = await OnPrepareInternals(OnMapModelToInternal(new[] { m }.AsContextQueryable()).ToArray());
				if (re == null || re.Length == 0)
					return default(TEntityDetail);
				return re[0];
			}
			else
				return default(TEntityDetail);
		}

		public async Task<TEntityDetail[]> GetAsync(TKey[] Ids)
		{
			var re = await OnPrepareInternals(
				OnMapModelToInternal(
					Ids.Select(id => Models.Get(id))
					.Where(m => m != null)
					.AsContextQueryable()
					).ToArray());
			return re;
		}
	}




}
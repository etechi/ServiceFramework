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
using System.Threading.Tasks;
using System.Collections.Generic;
using SF.Sys.Services;
using SF.Sys.Logging;
using SF.Sys.Data;
using SF.Sys.Linq;
using SF.Sys.Collections.Generic;

namespace SF.Sys.Entities
{
	public abstract class BaseEntityManager: IManagedServiceWithId
	{
		protected IEntityServiceContext ServiceContext { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor => ServiceContext.ServiceInstanceDescroptor;
		public DateTime Now => ServiceContext.Now;
		ILogger _Logger;
		public ILogger Logger
		{
			get
			{
				if (_Logger == null)
					_Logger = (ILogger)ServiceContext.ServiceProvider.GetService(typeof(ILogger<>).MakeGenericType(GetType()));
				return _Logger;
			}
		}
		public IIdentGenerator IdentGenerator => ServiceContext.IdentGenerator;
		
		public long? ServiceInstanceId => ServiceContext.ServiceInstanceDescroptor.InstanceId;
		public long? DataScopeId => ServiceContext.ServiceInstanceDescroptor.DataScopeId;

		public BaseEntityManager(IEntityServiceContext EntityManager)
		{
			this.ServiceContext = EntityManager;
		}
	}

	public abstract class BaseDataSetEntityManager<TModel>:
		BaseEntityManager
		where TModel:class
	{
		public IDataSet<TModel> DataSet => ServiceContext.DataContext.Set<TModel>();
		public IDataContext DataContext => DataSet.Context;

		public BaseDataSetEntityManager(IEntityServiceContext ServiceContext) :base(ServiceContext)
		{
		}
	}

	public abstract class EntitySource<TKey, TEntityDetail, TModel> :
		EntitySource<TKey, TEntityDetail, TEntityDetail, TModel>
		where TModel : class
	{
		public EntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
		protected override async Task<TEntityDetail[]> OnPrepareDetails(TEntityDetail[] Internals)
		{
			await ServiceContext.DataEntityResolver.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
			return Internals;
		}
	}
	
	public abstract class EntitySource<TKey,TDetail, TDetailTemp, TModel> :
		BaseDataSetEntityManager<TModel>,
		IEntityLoadable<TKey, TDetail>,
		IEntityBatchLoadable<TKey, TDetail>
		where TModel: class
	{
		public EntitySource(IEntityServiceContext ServiceContext) :base(ServiceContext)
		{
		}
		protected virtual IContextQueryable<TDetailTemp> OnMapModelToDetail(IContextQueryable<TModel> Query)
		{
			return Query.Select(Poco.MapExpression<TModel, TDetailTemp>());
		}
		protected abstract Task<TDetail[]> OnPrepareDetails(TDetailTemp[] Internals);

		public Task<TDetail[]> BatchGetAsync(TKey[] Ids)
		{
			return ServiceContext.BatchGetAsync<TKey, TDetailTemp,TDetail,TModel>(Ids, OnMapModelToDetail,OnPrepareDetails);
		}

		public Task<TDetail> GetAsync(TKey Id)
		{
			return ServiceContext.GetAsync<TKey, TDetailTemp, TDetail, TModel>(Id, OnMapModelToDetail, OnPrepareDetails);
		}
	}
	public abstract class AutoEntitySource<TKey, TDetail, TModel> :
	   BaseDataSetEntityManager<TModel>,
	   IEntityLoadable<TKey, TDetail>,
	   IEntityBatchLoadable<TKey, TDetail>
	   where TModel : class
	{
		public AutoEntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
		public virtual Task<TDetail[]> BatchGetAsync(TKey[] Ids)
		{
			return ServiceContext.AutoBatchGetAsync<TKey, TDetail, TModel>(Ids);
		}

		public virtual Task<TDetail> GetAsync(TKey Id)
		{
			return ServiceContext.AutoGetAsync<TKey, TDetail, TModel>(Id);
		}
	}
	public abstract class ConstantEntitySource<TKey, TEntityDetail> :
		ConstantEntitySource<TKey, TEntityDetail, TEntityDetail>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{
		public ConstantEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TEntityDetail> Models) : base(EntityManager, Models)
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
		public ConstantEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TModel> Models) : base(EntityManager, Models)
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
		public ConstantEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TModel> Models) :base(EntityManager)
		{
			this.Models = Models;
		}
		protected virtual IContextQueryable<TTemp> OnMapModelToInternal(IContextQueryable<TModel> Query)
		{
			return Query.Select(Poco.MapExpression<TModel, TTemp>());
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

		public async Task<TEntityDetail[]> BatchGetAsync(TKey[] Ids)
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
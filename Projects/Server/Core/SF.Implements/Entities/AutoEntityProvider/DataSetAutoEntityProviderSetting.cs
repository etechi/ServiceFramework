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
	class DataSetAutoEntityProviderSetting<TKey,TEntityDetail, TEntityDetailTemp, TEntitySummary, TEntitySummaryTemp, TEntityEditable, TEntityEditableTemp, TQueryArgument,TDataModel>
		: IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		where TDataModel : class,new()
	{
		Lazy<Func<IDataSetEntityManager<TEntityEditable,TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntityDetailTemp>>> FuncMapModelToDetailTemp { get; }
		Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, TEntityDetailTemp[], Task<TEntityDetail[]>>> FuncMapDetailTempToDetail { get; }

		Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntitySummaryTemp>>> FuncMapModelToSummaryTemp { get; }
		Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, TEntitySummaryTemp[], Task<TEntitySummary[]>>> FuncMapSummaryTempToSummary { get; }

		Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IContextQueryable<TDataModel>, TQueryArgument, Paging, IContextQueryable<TDataModel>>> FuncBuildQuery { get; }

		Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IContextQueryable<TDataModel>, Task<TEntityEditable>>> FuncLoadEditable { get; }
		Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, TKey, IContextQueryable<TDataModel>, Task<TDataModel>>> FuncLoadModelForModify { get; }
		Lazy<IPagingQueryBuilder<TDataModel>> PagingQueryBuilder { get; }

		IQueryResultBuildHelper<TDataModel, TEntityDetailTemp, TEntityDetail> DetailQueryResultBuildHelper { get; }
		IQueryResultBuildHelper<TDataModel, TEntitySummaryTemp, TEntitySummary> SummaryQueryResultBuildHelper { get; }
		IQueryResultBuildHelper<TDataModel, TEntityEditableTemp,TEntityEditable> EditableQueryResultBuildHelper { get; }
		IEntityModifier<TEntityEditable, TDataModel>[] InitModifiers { get; }
		IEntityModifier<TEntityEditable, TDataModel>[] UpdateModifiers { get; }
		IEntityModifier<TEntityEditable, TDataModel>[] RemoveModifiers { get; }

		public Lazy<Func<IServiceProvider, object>> FuncCreateProvider { get; }

		void ValidateEntityTypes(IMetadataCollection metas, IEntityType entity, params Type[] types)
		{
			foreach (var type in types)
			{
				var ne = metas.EntityTypesByType.Get(type);
				if (ne == null)
					throw new ArgumentException($"自动化实体类型库中找不到类型{type}对应的实体");

				if (ne != entity)
					throw new ArgumentException($"类型{type}对应的实体{ne.FullName}和类型{typeof(TEntityDetail)}对应的实体{entity.FullName}不一致");
			}
		}

		public Task<TKey> CreateAsync(IDataSetEntityManager EntityManager,TEntityEditable Entity)
		{
			var em = (IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager;
			return em.CreateAsync<TKey,TEntityEditable, TDataModel>(
				Entity,
				async ctx => {
					foreach (var m in InitModifiers)
						await m.Execute(em, ctx);
				},
				async ctx =>
				{
					foreach (var m in UpdateModifiers)
						await m.Execute(em, ctx);
				},
				null
				);
		}

		public Task<TEntityDetail> GetAsync(IDataSetEntityManager EntityManager, TKey Id)
		{
			var em = (IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager;
			return em.GetAsync(
				Id,
				ctx => FuncMapModelToDetailTemp.Value(em, ctx),
				items => FuncMapDetailTempToDetail.Value(em, items)
				);
		}

		public Task<TEntityDetail[]> GetAsync(IDataSetEntityManager EntityManager, TKey[] Ids)
		{
			var em = (IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager;
			return em.BatchGetAsync(
				Ids,
				ctx => FuncMapModelToDetailTemp.Value(em, ctx),
				items => FuncMapDetailTempToDetail.Value(em, items)
				);
		}

		public Task<TEntityEditable> LoadForEdit(IDataSetEntityManager EntityManager, TKey Id)
		{
			var em = ((IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager);
			return em.LoadForEdit(
				Id,
				(ctx) => FuncLoadEditable.Value(em, ctx)
				);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(IDataSetEntityManager EntityManager, TQueryArgument Arg, Paging paging)
		{
			var em = ((IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager);
			return em.QueryAsync<TEntitySummaryTemp, TEntitySummary, TQueryArgument, TDataModel>(
				Arg,
				paging,
				(ctx, args, pg) => FuncBuildQuery.Value(em, ctx, args, pg),
				PagingQueryBuilder.Value,
				(ctx) => FuncMapModelToSummaryTemp.Value(em, ctx),
				(items) => FuncMapSummaryTempToSummary.Value(em, items)
				);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(IDataSetEntityManager EntityManager, TQueryArgument Arg, Paging paging)
		{
			var em = ((IDataSetEntityManager<TEntityEditable, TDataModel>)EntityManager);
			return em.QueryIdentsAsync<TKey,TQueryArgument, TDataModel>(
				Arg,
				paging,
				(ctx, args, pg) => FuncBuildQuery.Value(em, ctx, args, pg),
				PagingQueryBuilder.Value
				);
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
				async ctx => {
					foreach (var m in RemoveModifiers)
						await m.Execute(em, ctx);
				},
				(key, ctx) => FuncLoadModelForModify.Value(em, key, ctx)
				);
		}

		public async Task UpdateAsync(IDataSetEntityManager EntityManager, TEntityEditable Entity)
		{
			var em = ((IDataSetEntityManager<TEntityEditable,TDataModel>)EntityManager);
			await em.UpdateAsync<TKey,TEntityEditable, TDataModel>(
				Entity,
				async ctx => {
					foreach (var m in UpdateModifiers)
						await m.Execute(em, ctx);
				},
				(key, ctx) => FuncLoadModelForModify.Value(em, key, ctx)
				);
		}

		public DataSetAutoEntityProviderSetting(
			IQueryResultBuildHelper<TDataModel,TEntityDetailTemp,TEntityDetail> DetailQueryResultBuildHelper,
			IQueryResultBuildHelper<TDataModel, TEntitySummaryTemp, TEntitySummary> SummaryQueryResultBuildHelper,
			IQueryResultBuildHelper<TDataModel, TEntityEditableTemp, TEntityEditable> EditableQueryResultBuildHelper,
			IEntityModifier<TEntityEditable, TDataModel>[] InitModifiers,
			IEntityModifier<TEntityEditable, TDataModel>[] UpdateModifiers,
			IEntityModifier<TEntityEditable, TDataModel>[] RemoveModifiers
		)
		{
			this.DetailQueryResultBuildHelper = DetailQueryResultBuildHelper;
			this.SummaryQueryResultBuildHelper = SummaryQueryResultBuildHelper;
			this.EditableQueryResultBuildHelper = EditableQueryResultBuildHelper;
			this.InitModifiers = InitModifiers;
			this.UpdateModifiers = UpdateModifiers;
			this.RemoveModifiers = RemoveModifiers;

			FuncCreateProvider = new Lazy<Func<IServiceProvider, object>>(() =>
				new Func<IServiceProvider, object>(
					sp=>new DataSetAutoEntityProvider<TKey,TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(
							sp.Resolve<IDataSetEntityManager<TEntityDetail,TDataModel>>(),
							this
						)
				)
			 );

			FuncMapModelToDetailTemp = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntityDetailTemp>>>(() =>
			{
				return (em, ctx) => ctx.Select(DetailQueryResultBuildHelper.EntityMapper);
			});
			FuncMapDetailTempToDetail = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, TEntityDetailTemp[], Task<TEntityDetail[]>>>(() =>
			{
				return async (em, tmps) =>
				{
					var re = await DetailQueryResultBuildHelper.ResultMapper(tmps);
					await EntityResolver.Fill(em.DataEntityResolver, em.ServiceInstanceDescroptor.InstanceId, re);
					return re;
				};
			});
			FuncMapModelToSummaryTemp = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntitySummaryTemp>>>(() =>
			{
				return (em, ctx) => ctx.Select(SummaryQueryResultBuildHelper.EntityMapper);
			});
			FuncMapSummaryTempToSummary = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, TEntitySummaryTemp[], Task<TEntitySummary[]>>>(() =>
			{
				return async (em, tmps) =>
				{
					var re = await SummaryQueryResultBuildHelper.ResultMapper(tmps);
					await EntityResolver.Fill(em.DataEntityResolver, em.ServiceInstanceDescroptor.InstanceId, re);
					return re;
				};
			});

			FuncBuildQuery = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IContextQueryable<TDataModel>, TQueryArgument, Paging, IContextQueryable<TDataModel>>>(() =>
			{
				return (em, q, arg, pg) => q;
			});

			FuncLoadEditable = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IContextQueryable<TDataModel>, Task<TEntityEditable>>>(() =>
			{
				return new Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IContextQueryable<TDataModel>, Task<TEntityEditable>>(
					async (IDataSetEntityManager<TEntityEditable, TDataModel>  em, IContextQueryable<TDataModel> q) =>
				{
					var re = await q.Select(EditableQueryResultBuildHelper.EntityMapper).SingleOrDefaultAsync();
					if (re == null)
						return default(TEntityEditable);
					var re1 = await EditableQueryResultBuildHelper.ResultMapper(new[] { re });
					if (re1 == null || re1.Length == 0)
						return default(TEntityEditable);
					return re1[0];
				});
			});

			FuncLoadModelForModify = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, TKey, IContextQueryable<TDataModel>, Task<TDataModel>>>(() =>
			{
				return (em,e, q) => q.SingleOrDefaultAsync();
			});

			//FuncInitModel = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IEntityModifyContext<TEntityEditable, TDataModel>, Task>>(() =>
			//{
			//	return async (em, ctx) =>
			//		{
			//			foreach (var m in InitModifiers)
			//				await m.Execute(em, ctx);
			//		};
			//});

			//FuncUpdateModel = new Lazy<Func<IDataSetEntityManager<TEntityEditable, TDataModel>, IEntityModifyContext<TEntityEditable, TDataModel>, Task>>(() =>
			//{
			//	return async (em, ctx) =>
			//	{
			//		foreach (var m in InitModifiers)
			//			await m.Execute(em, ctx);
			//	};

			//	//var ArgModel = Expression.Parameter(typeof(TDataModel));
			//	//var ArgEditable = Expression.Parameter(typeof(TEntityEditable));
			//	//var update = Expression.Lambda<Action<TDataModel, TEntityEditable>>(
			//	//	Expression.Block(
			//	//		from dstProp in typeof(TDataModel).AllPublicInstanceProperties()
			//	//		where dstProp.CanWrite
			//	//		let srcProp = typeof(TEntityEditable).GetProperty(dstProp.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
			//	//		where srcProp != null
			//	//		select Expression.Call(
			//	//			ArgModel,
			//	//			dstProp.SetMethod,
			//	//			Expression.Property(ArgEditable, srcProp)
			//	//			)
			//	//	),
			//	//	ArgModel,
			//	//	ArgEditable
			//	//	).Compile();

			//	//return (em, ctx) =>
			//	//{
			//	//	update(ctx.Model, ctx.Editable);
			//	//	return Task.CompletedTask;
			//	//};
			//});

			//FuncRemoveModel = new Lazy<Func<IDataSetEntityManager<TEntityEditable,TDataModel>, IEntityModifyContext<TEntityEditable, TDataModel>, Task>>(() =>
			//{
			//	return (em, ctx) => Task.CompletedTask;
			//});

			PagingQueryBuilder = new Lazy<IPagingQueryBuilder<TDataModel>>(() =>
				  SummaryQueryResultBuildHelper.PagingBuilder
			);
		}
	}


}

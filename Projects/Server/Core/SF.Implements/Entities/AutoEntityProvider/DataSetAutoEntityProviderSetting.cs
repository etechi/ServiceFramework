using System;
using System.Reflection;
using System.Linq.Expressions;
using SF.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SF.Entities.AutoEntityProvider
{
	class DataSetAutoEntityProviderSetting
	{
		public Lazy<Func<IServiceProvider, object>> FuncCreateProvider;

	}

	class DataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntityDetailTemp, TEntitySummary, TEntitySummaryTemp, TEntityEditable, TQueryArgument,TDataModel>
		: DataSetAutoEntityProviderSetting
		where TEntityDetail : class, IEntityWithId<TKey>
		where TEntitySummary : class, IEntityWithId<TKey>
		where TEntityEditable : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TQueryArgument : IQueryArgument<TKey>
		where TDataModel : class,IEntityWithId<TKey>,new()
	{
		public Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntityDetailTemp>>> FuncMapModelToDetailTemp;
		public Lazy<Func<IDataSetEntityManager<TDataModel>, TEntityDetailTemp[], Task<TEntityDetail[]>>> FuncMapDetailTempToDetail;

		public Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntitySummaryTemp>>> FuncMapModelToSummaryTemp;
		public Lazy<Func<IDataSetEntityManager<TDataModel>, TEntitySummaryTemp[], Task<TEntitySummary[]>>> FuncMapSummaryTempToSummary;

		public Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, TQueryArgument, Paging, IContextQueryable<TDataModel>>> FuncBuildQuery;

		public Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, Task<TEntityEditable>>> FuncLoadEditable;
		public Lazy<Func<IDataSetEntityManager<TDataModel>, TKey, IContextQueryable<TDataModel>, Task<TDataModel>>> FuncLoadModelForModify;
		public Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TEntityEditable, TDataModel>, Task>> FuncInitModel;
		public Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TEntityEditable, TDataModel>, Task>> FuncUpdateModel;
		public Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TDataModel>, Task>> FuncRemoveModel;

		public Lazy<PagingQueryBuilder<TDataModel>> PagingQueryBuilder;

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
		public DataSetAutoEntityProviderSetting(IMetadataCollection metas)
		{
			var entity = metas.EntityTypesByType.Get(typeof(TEntityDetail));
			if (entity == null)
				throw new ArgumentException($"自动化实体类型库中找不到类型{typeof(TEntityDetail)}对应的实体");
			ValidateEntityTypes(
				metas,
				entity,
				typeof(TEntityDetailTemp),
				typeof(TEntitySummary),
				typeof(TEntitySummaryTemp),
				typeof(TEntityEditable)
				);

			 FuncCreateProvider = new Lazy<Func<IServiceProvider, object>>(() =>
			 {
				 Type TEntitySummaryTemp = null;
				 Type TEntityDetailTemp = null;
				 Type DataModel = null;


				 var spArg = Expression.Parameter(typeof(IServiceProvider));

				 return Expression.Lambda<Func<IServiceProvider, object>>(
					 Expression.New(
						 typeof(DataSetAutoEntityProviderSetting<,,,,,,,>).MakeGenericType(
							 typeof(TKey), 
							 typeof(TEntityDetail), 
							 TEntityDetailTemp, 
							 typeof(TEntitySummary), 
							 TEntitySummaryTemp, 
							 typeof(TEntityEditable), 
							 typeof(TQueryArgument), 
							 DataModel
							 ).GetConstructor(new[] { typeof(IServiceProvider) }),
						 spArg
						 ),
					 spArg
					 ).Compile();
			 });

			FuncMapModelToDetailTemp = new Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntityDetailTemp>>>(() =>
			{
				//var ArgModel = Expression.Parameter(typeof(TDataModel));
				//var expr = Expression.Lambda<Func<TDataModel, TEntityDetailTemp>>(
				//		Expression.MemberInit(
				//		Expression.New(typeof(TEntityDetailTemp)),
				//		from dstProp in typeof(TEntityDetailTemp).AllPublicInstanceProperties()
				//		let srcProp = typeof(TDataModel).GetProperty(dstProp.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
				//		where srcProp != null
				//		select Expression.Bind(dstProp, Expression.Property(ArgModel, srcProp))
				//		),
				//		ArgModel
				//	);
				var expr = EntityMapper.Map<TDataModel, TEntityDetailTemp>();
				return (em, ctx) => ctx.Select(expr);
			});
			FuncMapDetailTempToDetail = new Lazy<Func<IDataSetEntityManager<TDataModel>, TEntityDetailTemp[], Task<TEntityDetail[]>>>(() =>
			{
				return (em, tmps) =>
				Task.FromResult(
					tmps.Select(tmp => EntityMapper.Map<TEntityDetailTemp, TEntityDetail>(tmp)).ToArray()
					);
			});
			FuncMapModelToSummaryTemp = new Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntitySummaryTemp>>>(() =>
			{
				var expr = EntityMapper.Map<TDataModel, TEntitySummaryTemp>();
				return (em, ctx) => ctx.Select(expr);
			});
			FuncMapSummaryTempToSummary = new Lazy<Func<IDataSetEntityManager<TDataModel>, TEntitySummaryTemp[], Task<TEntitySummary[]>>>(() =>
			{
				return (em, tmps) =>
				Task.FromResult(
					tmps.Select(tmp => EntityMapper.Map<TEntitySummaryTemp, TEntitySummary>(tmp)).ToArray()
					);
			});

			FuncBuildQuery = new Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, TQueryArgument, Paging, IContextQueryable<TDataModel>>>(() =>
			{
				return (em, q, arg, pg) => q;
			});

			FuncLoadEditable = new Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, Task<TEntityEditable>>>(() =>
			{
				var expr = EntityMapper.Map<TDataModel, TEntityEditable>();
				return (em, q) =>
				  q.Select(expr).SingleOrDefaultAsync();
			});

			FuncLoadModelForModify = new Lazy<Func<IDataSetEntityManager<TDataModel>, TKey, IContextQueryable<TDataModel>, Task<TDataModel>>>(() =>
			{
				return (em, id, q) => q.SingleOrDefaultAsync();
			});

			FuncInitModel = new Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TEntityEditable, TDataModel>, Task>>(() =>
			{
				return (em, ctx) => Task.CompletedTask;
			});

			FuncUpdateModel = new Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TEntityEditable, TDataModel>, Task>>(() =>
			{
				var ArgModel = Expression.Parameter(typeof(TDataModel));
				var ArgEditable = Expression.Parameter(typeof(TEntityEditable));
				var update = Expression.Lambda<Action<TDataModel, TEntityEditable>>(
					Expression.Block(
						from dstProp in typeof(TDataModel).AllPublicInstanceProperties()
						where dstProp.CanWrite
						let srcProp = typeof(TEntityEditable).GetProperty(dstProp.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
						where srcProp != null
						select Expression.Call(
							ArgModel,
							dstProp.SetMethod,
							Expression.Property(ArgEditable, srcProp)
							)
					),
					ArgModel,
					ArgEditable
					).Compile();

				return (em, ctx) =>
				{
					update(ctx.Model, ctx.Editable);
					return Task.CompletedTask;
				};
			});

			FuncRemoveModel = new Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TDataModel>, Task>>(() =>
			{
				return (em, ctx) => Task.CompletedTask;
			});


			PagingQueryBuilder = new Lazy<PagingQueryBuilder<TDataModel>>(() =>
				  new PagingQueryBuilder<TDataModel>("id", b => b.Add("id", m => m.Id))
			);
		}
	}


}

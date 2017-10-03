using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Data;
using SF.Entities;
using SF.Core.ServiceManagement;
using SF.Core.Times;
using SF.Common.Documents.DataModels;

namespace SF.Common.Documents.Management
{
	public class DocumentCategoryManager :
		DocumentCategoryManager<CategoryInternal, DataModels.Document, DataModels.DocumentAuthor, DataModels.DocumentCategory, DataModels.DocumentTag, DataModels.DocumentTagReference>,
		IDocumentCategoryManager
	{
		public DocumentCategoryManager(IDataSetEntityManager<CategoryInternal,DocumentCategory> EntityManager) : base(EntityManager)
		{
		}
	}

	public class DocumentCategoryManager<TCategoryInternal, TDocument, TAuthor, TCategory, TTag, TTagReference> :
		ModidifiableEntityManager<ObjectKey<long>, TCategoryInternal, DocumentCategoryQueryArgument,TCategoryInternal, TCategory>,
		IDocumentCategoryManager<TCategoryInternal>
		where TCategoryInternal:CategoryInternal,new()
		where TDocument : DataModels.Document<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DataModels.DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DataModels.DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>, new()
		where TTag : DataModels.DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DataModels.DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{
		protected override PagingQueryBuilder<TCategory> PagingQueryBuilder => new PagingQueryBuilder<TCategory>(
			"order", b => b.Add("order", c => c.ItemOrder)
			);

		protected override async Task<TCategoryInternal> OnMapModelToEditable (IContextQueryable<TCategory> Query)
		{
			return await OnMapModelToDetail(Query).SingleOrDefaultAsync();
		}
        protected override IContextQueryable<TCategoryInternal> OnMapModelToDetail(IContextQueryable<TCategory> Query)
		{
			return Query.SelectUIObjectEntity<TCategory,TCategoryInternal>(c=>
				new TCategoryInternal
				{
					Id=c.Id,
					ContainerId=c.ContainerId,
					ItemOrder=c.ItemOrder
				}
				);
		}

		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;

			Model.Id = obj.Id;

			if (obj.ContainerId != Model.ContainerId)
			{
				var cats = DataSet.Context.Set<TCategory>().AsQueryable();
				await DataSet.ValidateTreeParentAsync(
					"文档分类",
					Model.Id,
					obj.ContainerId?? 0,
					pnt => cats
						.Where(c => c.Id == pnt)
						.Select(c => c.ContainerId.HasValue ? c.ContainerId.Value : 0)
					);

				Model.ContainerId = obj.ContainerId;
			}
			Model.Update(obj, Now);
		}

		public DocumentCategoryManager(
			IDataSetEntityManager<TCategoryInternal,TCategory> EntityManager
			) : base(EntityManager)
		{

		}

		protected override Task OnNewModel(IModifyContext ctx)
		{
			return base.OnNewModel(ctx);
		}
		protected override IContextQueryable<TCategory> OnBuildQuery(IContextQueryable<TCategory> Query, DocumentCategoryQueryArgument Arg, Paging paging)
		{
			var q = Query.WithScope(ServiceInstanceDescriptor)
				.Filter(Arg.ParentId, c => c.ContainerId)
				.FilterContains(Arg.Name,c=>c.Name);
			return q;

		}
	}
}

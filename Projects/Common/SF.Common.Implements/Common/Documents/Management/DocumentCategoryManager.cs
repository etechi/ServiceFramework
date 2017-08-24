using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

using ServiceProtocol.Data.Entity;

namespace ServiceProtocol.Biz.Documents.Entity
{
    [DataObjectLoader("文档目录")]
	public class DocumentCategoryManager<TCategoryInternal,TDocument, TAuthor, TCategory, TTag, TTagReference, TScope> :
		ObjectManager.EntityServiceObjectManager<int, TCategoryInternal, TCategoryInternal, TCategory>,
		ServiceProtocol.Biz.Documents.IDocumentCategoryManager<TCategoryInternal>,
        IDataObjectLoader
		where TCategoryInternal:CategoryInternal,new()
		where TDocument : Models.Document<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
		where TAuthor : Models.DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
		where TCategory : Models.DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>, new()
		where TTag : Models.DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
		where TTagReference : Models.DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
		where TScope : Models.DocumentScope<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
	{
        protected override async Task<TCategoryInternal> MapModelToEditable(IContextQueryable<TCategory> Query)
		{
			return await Query.Select(s => new TCategoryInternal
			{
				Id=s.Id,

				ParentId=s.ParentId,

				Name =s.Name,
				Image=s.Image,
				Order=s.Order,
				Summary =s.Summary,
				ScopeId=s.ScopeId,

				ObjectState =s.ObjectState
			}).SingleOrDefaultAsync();
		}

        protected override IContextQueryable<TCategoryInternal> MapModelToInternal(IContextQueryable<TCategory> Query)
		{
			return Query.Select(s => new TCategoryInternal
			{
				Id = s.Id,

				ParentId = s.ParentId,
				ParentName = s.ParentId == null ? null : s.Parent.Name,

				Name = s.Name,
				Image = s.Image,
				Order = s.Order,
				Summary = s.Summary,
				ScopeId = s.ScopeId,
				ScopeName = s.Scope.Name,

				ObjectState = s.ObjectState
			});
		}

		protected override async Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;

			Model.Id = obj.Id;

			if (obj.ParentId != Model.ParentId)
			{
				await Context.ValidateTreeParent(
					"文档分类",
					Model.Id,
					obj.ParentId ?? 0,
					pnt => Context.ReadOnly<TCategory>()
						.Where(c => c.Id == pnt)
						.Select(c => c.ParentId.HasValue ? c.ParentId.Value : 0)
					);

				Model.ParentId = obj.ParentId;
			}
			Model.Name = obj.Name;
			Model.Image = obj.Image;
			Model.Order = obj.Order;
			Model.Summary = obj.Summary;
			Model.ScopeId = obj.ScopeId;

			Model.ObjectState = obj.ObjectState;

		}
		public DocumentCategoryManager(IDataContext context,Lazy<IModifyFilter> ModifyFilter) : base(context, ModifyFilter)
		{
		}

		public async Task<TCategoryInternal[]> List(ListQueryArgument Argument)
		{
			var q = (IContextQueryable<TCategory>)Context.ReadOnly<TCategory>();
			if (Argument != null)
			{
				if (Argument.ScopeId != null)
					q = q.Where(m => m.ScopeId == Argument.ScopeId);
				if (Argument.ParentId != null)
					q = q.Where(m => m.ParentId == Argument.ParentId);
			}
				
			return await MapModelToInternal(q).ToArrayAsync();
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            var re = await DataObjectLoader.Load(
                Keys,
                id => int.Parse(id[0]),
                id => FindByIdAsync(id),
                async (ids) => {
                    var tmps = await MapModelToInternal(Context.ReadOnly<TCategory>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
                    return tmps;
                });
            return re;
        }

    }
}

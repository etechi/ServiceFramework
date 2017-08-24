using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

using ServiceProtocol.Data.Entity;

namespace ServiceProtocol.Biz.Documents.Entity
{
    [DataObjectLoader("文档")]
    public class DocumentManager<TInternal,TEditable,TDocument, TAuthor, TCategory, TTag, TTagReference, TScope> :
		ObjectManager.EntityServiceObjectManager<int, TInternal, TEditable, TDocument>,
		ServiceProtocol.Biz.Documents.IDocumentManager<TInternal, TEditable>,
        IDataObjectLoader
		where TInternal:DocumentInternal,new()
		where TEditable: DocumentEditable, new()
		where TDocument : Models.Document<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>,new()
		where TAuthor : Models.DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
		where TCategory : Models.DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
		where TTag : Models.DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
		where TTagReference : Models.DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
		where TScope : Models.DocumentScope<TDocument, TAuthor, TCategory, TTag, TTagReference, TScope>
	{
        protected override async Task<TEditable> MapModelToEditable(IContextQueryable<TDocument> Query)
		{
			return await Query.Select(s => new TEditable
			{
				Id = s.Id,
				Name = s.Name,
				Ident = s.Ident,
				CategoryId = s.CategoryId,
				PublishDate = s.PublishDate,
				Order = s.Order,
				ScopeId = s.ScopeId,
				ObjectState = s.ObjectState,
				Image = s.Image,
				Content = s.Content,

			}).SingleOrDefaultAsync();
		}

        protected override IContextQueryable<TInternal> MapModelToInternal(IContextQueryable<TDocument> Query)
		{
			return Query.Select(s => new TInternal
			{
				Id = s.Id,
				Name = s.Name,
				Ident = s.Ident,
				CategoryId = s.CategoryId,
				CategoryName=s.CategoryId==null?null:s.Category.Name,
				PublishDate = s.PublishDate,
				Order = s.Order,
				ScopeId = s.ScopeId,
				ScopeName=s.Scope.Name,
				ObjectState = s.ObjectState,
			});
		}

		protected override Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;

			Model.Id = obj.Id;
			Model.Name = obj.Name;
			Model.Ident = obj.Ident;
			Model.CategoryId = obj.CategoryId;
			Model.PublishDate = obj.PublishDate;
			Model.Order = obj.Order;
			Model.ScopeId = obj.ScopeId;
			Model.ObjectState = obj.ObjectState;
			Model.Image = obj.Image;
			Model.Content = obj.Content;

			return Task.CompletedTask;
		}

		static PagingQueryBuilder<TDocument> pagingBuilder = new PagingQueryBuilder<TDocument>(
			"name",
			i => i.Add("name", m => m.Name)
			.Add("date",m=>m.PublishDate,true)
			.Add("order", m => m.Order)
			);
		public async Task<QueryResult<TInternal>> Query(DocumentQueryArguments args, Paging paging)
		{
			var q = (IContextQueryable<TDocument>)Context.ReadOnly<TDocument>();
			if(args.ScopeId!=null)
				q = q.Where(c => c.ScopeId == args.ScopeId);
			if (args.CategoryId != null)
				q = q.Where(c => c.CategoryId == args.CategoryId);
			if(args.Name!=null)
				q = q.Where(c => c.Name.Contains(args.Name));

			if(args.PublishDate!=null)
			{
				if (args.PublishDate.Begin != null)
					q = q.Where(c => c.PublishDate >= args.PublishDate.Begin);
				if (args.PublishDate.End != null)
					q = q.Where(c => c.PublishDate <= args.PublishDate.End);
			}

			return await q.ToQueryResultAsync(
				MapModelToInternal,
				r => r,
				pagingBuilder,
				paging
				);
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            return await DataObjectLoader.Load(
                Keys,
                id => int.Parse(id[0]),
                id => FindByIdAsync(id),
                async (ids) => {
                    var tmps = await MapModelToInternal(
                        Context.ReadOnly<TDocument>().Where(a => ids.Contains(a.Id))
                        ).ToArrayAsync();
                    return await OnPrepareInternals(tmps);
                }
                );
        }

        public DocumentManager(IDataContext context,Lazy<IModifyFilter> ModifyFilter) : base(context, ModifyFilter)
		{
		}

	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
using SF.Core.Times;
using SF.Data;
using SF.Core.ServiceManagement;

namespace SF.Common.Documents.Management
{
	public class DocumentManager :
		DocumentManager<DocumentInternal, DocumentEditable, DataModels.Document, DataModels.DocumentAuthor, DataModels.DocumentCategory, DataModels.DocumentTag, DataModels.DocumentTagReference>,
		IDocumentManager
	{
		public DocumentManager(IDataSetEntityManager<DocumentEditable, DataModels.Document> EntityManager) : base(EntityManager)
		{
		}
	}
	public class DocumentManager<TInternal,TEditable,TDocument, TAuthor, TCategory, TTag, TTagReference> :
		ModidifiableEntityManager<ObjectKey<long>, TInternal, DocumentQueryArguments,TEditable,TDocument>,
		IDocumentManager<TInternal,TEditable>
		where TInternal: DocumentInternal,new()
		where TEditable: DocumentEditable, new()
		where TDocument : DataModels.Document<TDocument, TAuthor, TCategory, TTag, TTagReference>,new()
		where TAuthor : DataModels.DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DataModels.DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DataModels.DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DataModels.DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{
		
		public DocumentManager(IDataSetEntityManager<TEditable,TDocument> Manager) : base(Manager)
		{
		}
		protected override async Task<TEditable> OnMapModelToEditable(IContextQueryable<TDocument> Query)
		{
			return await Query.Select(
				ADT.Poco.Map<TDocument,TEditable>()
				).SingleOrDefaultAsync();
		}

		protected override IContextQueryable<TInternal> OnMapModelToDetail(IContextQueryable<TDocument> Query)
		{
			return Query.Select(ADT.Poco.Map<TDocument,TInternal>());
		}

		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Update(obj, Now);
			
			Model.Ident = obj.Ident;
			Model.PublishDate = obj.PublishDate;
			Model.Image = obj.Image;
			Model.Content = obj.Content;
			return Task.CompletedTask;
		}
		protected override PagingQueryBuilder<TDocument> PagingQueryBuilder => new PagingQueryBuilder<TDocument>(
			"name",
			i => i.Add("name", m => m.Name)
			.Add("date", m => m.PublishDate, true)
			.Add("order", m => m.ItemOrder)
			);


		protected override IContextQueryable<TDocument> OnBuildQuery(IContextQueryable<TDocument> Query, DocumentQueryArguments args, Paging paging)
		{
			var q = DataSet.AsQueryable().WithScope(ServiceInstanceDescriptor);
			q.Filter(args.CategoryId, c => c.ContainerId)
				.FilterContains(args.Name, c => c.Name)
				.Filter(args.CategoryId, c => c.ContainerId)
				.Filter(args.PublishDate, c => c.PublishDate.Value);
			return q;
				
		}
	}
}

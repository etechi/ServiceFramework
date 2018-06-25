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
using SF.Common.Documents.DataModels;
using SF.Sys.Data;
using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys;

namespace SF.Common.Documents
{
	public class DocumentService : 
		DocumentService<Document, Category, DataDocumentScope, DataModels.DataDocument, DataModels.DataDocumentAuthor, DataModels.DataDocumentCategory, DataModels.DataDocumentTag, DataModels.DataDocumentTagReference>,
		IDocumentService
	{
		public DocumentService(IDataScope DataScope, IServiceInstanceDescriptor ServiceInstanceDescriptor) : base(DataScope, ServiceInstanceDescriptor)
		{
		}
	}

	public class DocumentService<TDocumentPublic, TCategoryPublic, TScope, TDocument, TAuthor, TCategory, TTag, TTagReference> :
		IDocumentService<TDocumentPublic, TCategoryPublic>
		where TDocumentPublic : Document, new()
		where TCategoryPublic : Category, new()
		where TScope : DataModels.DataDocumentScope<TScope,TDocument, TAuthor, TCategory, TTag, TTagReference>, new()
		where TDocument : DataModels.DataDocument<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>, new()
		where TAuthor : DataModels.DataDocumentAuthor<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DataModels.DataDocumentCategory<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DataModels.DataDocumentTag<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DataModels.DataDocumentTagReference<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>

	{
		public IDataScope DataScope { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public DocumentService(IDataScope DataScope, IServiceInstanceDescriptor ServiceInstanceDescriptor)
		{
			this.DataScope = DataScope;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}
		protected virtual IQueryable<TDocumentPublic> MapModelToPublic(IQueryable<TDocument> query, bool detail)
		{
			return query.SelectUIObjectEntity(
				m => new TDocumentPublic
				{
					ContainerId=m.ContainerId,
					Id = m.Id,
					Redirect=m.Redirect,
					Content = detail ? m.Content : null,
					Container = m.ContainerId.HasValue ? new TCategoryPublic { Id = m.ContainerId.Value } : null,
				});

		}
		protected virtual IQueryable<TCategoryPublic> MapModelToPublic(IQueryable<TCategory> query, bool detail)
		{
			return query.SelectUIObjectEntity(
				m => new TCategoryPublic
				{
					ContainerId = m.ContainerId,
					Id = m.Id,
				});
			
		}


		static PagingQueryBuilder<TDocument> docPageBuilder = new PagingQueryBuilder<TDocument>(
			"order",
			i => i.Add("order", d => d.ItemOrder));
		static PagingQueryBuilder<TCategory> catPageBuilder = new PagingQueryBuilder<TCategory>(
			"order",
			i => i.Add("order", d => d.ItemOrder));


		IQueryable<TDocument> LimitedDocuments(IDataContext ctx) => ctx.Set<TDocument>().AsQueryable()
				//.WithScope(ServiceInstanceDescriptor)
				.IsEnabled();

		IQueryable<TCategory> LimitedCategories(IDataContext ctx) => ctx.Set<TCategory>().AsQueryable()
				//.WithScope(ServiceInstanceDescriptor)
				.IsEnabled();

		public long? ServiceInstanceId => ServiceInstanceDescriptor.InstanceId;

		public Task<TDocumentPublic> GetDocument(long Id)
		{
			return DataScope.Use("获取文档", ctx => {
				var q = ctx.Queryable<TDocument>()
					.Where(i => i.Id == Id)
					//.EnabledScopedById(Id, ServiceInstanceDescriptor)
					;

				return MapModelToPublic(q, true)
					.SingleOrDefaultAsync();
				});
		}

		public Task<TDocumentPublic> GetDocumentByKey(string Id,string Scope)
		{
			if (Id.IsNullOrEmpty())
				throw new PublicArgumentException("没有文档标识");
			if (Scope == null) Scope = "default";
			return DataScope.Use("以标签获取文档", ctx => {
				var q = LimitedDocuments(ctx)
				.Where(d => d.Ident == Id && d.ScopeId==Scope);
				return MapModelToPublic(q, true).SingleOrDefaultAsync();
				});
		}

		public Task<TCategoryPublic> GetCategory(long Id)
		{
			return DataScope.Use("获取分类", ctx => {
				var q = LimitedCategories(ctx)
					.Where(i => i.Id == Id)
				//.EnabledScopedById(Id, ServiceInstanceDescriptor)
				;
				return MapModelToPublic(q, true).SingleOrDefaultAsync();
				});
		}

		public Task<QueryResult<TDocumentPublic>> Search(SearchArgument Arg)
		{
			return DataScope.Use("查询文档", ctx =>
			{
				var q = LimitedDocuments(ctx);
				var scope = Arg.Scope ?? "default";
				q = q.Where(d => d.ScopeId == scope);

				if (!string.IsNullOrWhiteSpace(Arg.Key))
					q = q.Where(
						d =>
						d.Name.Contains(Arg.Key) ||
						d.Description.Contains(Arg.Key) ||
						d.Content.Contains(Arg.Key) ||
						d.SubTitle.Contains(Arg.Key) ||
						d.Remarks.Contains(Arg.Key)
						);

				return q.ToQueryResultAsync(
					iq => MapModelToPublic(iq, false),
					r => r,
					docPageBuilder,
					Arg.Paging
					);
			});
		}

		public Task<QueryResult<TDocumentPublic>> ListDocuments(ListArgument Arg)
		{
			return DataScope.Use("文档列表", ctx =>
			{
				var q = LimitedDocuments(ctx);
				q = q.Where(d => d.ContainerId == Arg.Category );
				var scope = Arg.Scope ?? "default";
				q = q.Where(d => d.ScopeId == scope);
				return q.ToQueryResultAsync(
					iq => MapModelToPublic(iq, false),
					r => r,
					docPageBuilder,
					Arg.Paging
					);
			});
		}

		public Task<QueryResult<TCategoryPublic>> ListCategories(ListArgument Arg)
		{
			return DataScope.Use("目录列表", ctx =>
			{
				var q = LimitedCategories(ctx).Where(d =>
					d.ContainerId == Arg.Category
				);
				var scope = Arg.Scope ?? "default";
				q = q.Where(d => d.ScopeId == scope);
				return q.ToQueryResultAsync(
					iq => MapModelToPublic(q, false),
					r => r,
					catPageBuilder,
					Arg.Paging
					);
			});
		}
	}
}

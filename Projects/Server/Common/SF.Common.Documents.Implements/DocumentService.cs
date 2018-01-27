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

namespace SF.Common.Documents
{
	public class DocumentService : 
		DocumentService<Document, Category, DataModels.DataDocument, DataModels.DataDocumentAuthor, DataModels.DataDocumentCategory, DataModels.DocumentTag, DataModels.DocumentTagReference>,
		IDocumentService
	{
		public DocumentService(IDataScope DataScope, IServiceInstanceDescriptor ServiceInstanceDescriptor) : base(DataScope, ServiceInstanceDescriptor)
		{
		}
	}

	public class DocumentService<TDocumentPublic, TCategoryPublic, TDocument, TAuthor, TCategory, TTag, TTagReference> :
		IDocumentService<TDocumentPublic, TCategoryPublic>
		where TDocumentPublic : Document, new()
		where TCategoryPublic : Category, new()
		where TDocument : DataModels.DataDocument<TDocument, TAuthor, TCategory, TTag, TTagReference>, new()
		where TAuthor : DataModels.DataDocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DataModels.DataDocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DataModels.DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DataModels.DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>

	{
		public IDataScope DataScope { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public DocumentService(IDataScope DataScope, IServiceInstanceDescriptor ServiceInstanceDescriptor)
		{
			this.DataScope = DataScope;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}
		protected virtual IContextQueryable<TDocumentPublic> MapModelToPublic(IContextQueryable<TDocument> query, bool detail)
		{
			return query.SelectUIObjectEntity(
				m => new TDocumentPublic
				{
					ContainerId=m.ContainerId,
					Id = m.Id,
					Content = detail ? m.Content : null,
					Container = m.ContainerId.HasValue ? new TCategoryPublic { Id = m.ContainerId.Value } : null,
				});

		}
		protected virtual IContextQueryable<TCategoryPublic> MapModelToPublic(IContextQueryable<TCategory> query, bool detail)
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


		IContextQueryable<TDocument> LimitedDocuments(IDataContext ctx) => ctx.Set<TDocument>().AsQueryable()
				.WithScope(ServiceInstanceDescriptor)
				.IsEnabled();

		IContextQueryable<TCategory> LimitedCategories(IDataContext ctx) => ctx.Set<TCategory>().AsQueryable()
				.WithScope(ServiceInstanceDescriptor)
				.IsEnabled();

		public long? ServiceInstanceId => ServiceInstanceDescriptor.InstanceId;

		public Task<TDocumentPublic> GetAsync(ObjectKey<long> Id,string[] Fields=null)
		{
			return DataScope.Use("获取文档", ctx => {
				var q = ctx.Queryable<TDocument>()
					.Where(i => i.Id == Id.Id)
					.EnabledScopedById(Id.Id, ServiceInstanceDescriptor)
					;

				return MapModelToPublic(q, true)
					.SingleOrDefaultAsync();
				});
		}

		public Task<TDocumentPublic> GetByKeyAsync(string Key)
		{
			return DataScope.Use("以标签获取文档", ctx => {
				var q = LimitedDocuments(ctx)
				.Where(d => d.Ident == Key);
				return MapModelToPublic(q, true).SingleOrDefaultAsync();
				});
		}

		public Task<TCategoryPublic> LoadContainerAsync(long Key)
		{
			return DataScope.Use("获取分类", ctx => {
				var q = LimitedCategories(ctx)
					.Where(i => i.Id == Key)
				.EnabledScopedById(Key, ServiceInstanceDescriptor)
				;
				return MapModelToPublic(q, true).SingleOrDefaultAsync();
				});
		}

		public Task<QueryResult<TDocumentPublic>> SearchAsync(SearchArgument Arg)
		{
			return DataScope.Use("查询文档", ctx =>
			{
				var q = LimitedDocuments(ctx);
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

		public Task<QueryResult<TDocumentPublic>> ListItemsAsync(ListItemsArgument<long?> Arg)
		{
			return DataScope.Use("文档列表", ctx =>
			{
				var q = LimitedDocuments(ctx);
				if (Arg.Container.HasValue)
					q = q.Where(d =>
						  d.ContainerId == Arg.Container
						);
				return q.ToQueryResultAsync(
					iq => MapModelToPublic(iq, false),
					r => r,
					docPageBuilder,
					Arg.Paging
					);
			});
		}

		public Task<QueryResult<TCategoryPublic>> ListChildContainersAsync(ListItemsArgument<long?> Arg)
		{
			return DataScope.Use("文档列表", ctx =>
			{
				var q = LimitedCategories(ctx).Where(d =>
				d.ContainerId == Arg.Container
				);
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

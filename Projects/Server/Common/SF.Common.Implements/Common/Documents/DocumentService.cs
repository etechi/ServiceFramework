﻿#region Apache License Version 2.0
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

using SF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
using SF.Core.ServiceManagement;
using SF.Common.Documents.DataModels;

namespace SF.Common.Documents
{
	public class DocumentService : 
		DocumentService<Document, Category, DataModels.Document, DataModels.DocumentAuthor, DataModels.DocumentCategory, DataModels.DocumentTag, DataModels.DocumentTagReference>,
		IDocumentService
	{
		public DocumentService(Lazy<IDataSet<DataModels.Document>> Documents, Lazy<IDataSet<DocumentCategory>> Categories, IServiceInstanceDescriptor ServiceInstanceDescriptor) : base(Documents, Categories, ServiceInstanceDescriptor)
		{
		}
	}

	public class DocumentService<TDocumentPublic, TCategoryPublic, TDocument, TAuthor, TCategory, TTag, TTagReference> :
		IDocumentService<TDocumentPublic, TCategoryPublic>
		where TDocumentPublic : Document, new()
		where TCategoryPublic : Category, new()
		where TDocument : DataModels.Document<TDocument, TAuthor, TCategory, TTag, TTagReference>, new()
		where TAuthor : DataModels.DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DataModels.DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DataModels.DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DataModels.DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>

	{
		public Lazy<IDataSet<TDocument>> Documents { get; }
		public Lazy<IDataSet<TCategory>> Categories { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public DocumentService(Lazy<IDataSet<TDocument>> Documents, Lazy<IDataSet<TCategory>> Categories, IServiceInstanceDescriptor ServiceInstanceDescriptor)
		{
			this.Documents = Documents;
			this.Categories = Categories;
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


		IContextQueryable<TDocument> LimitedDocuments => Documents.Value.AsQueryable()
				.WithScope(ServiceInstanceDescriptor)
				.IsEnabled();

		IContextQueryable<TCategory> LimitedCategories => Categories.Value.AsQueryable()
				.WithScope(ServiceInstanceDescriptor)
				.IsEnabled();

		public long? ServiceInstanceId => ServiceInstanceDescriptor.InstanceId;

		public async Task<TDocumentPublic> GetAsync(ObjectKey<long> Id)
		{
			var q = Documents.Value.AsQueryable()
				.Where(i=>i.Id==Id.Id)
				.EnabledScopedById(Id.Id, ServiceInstanceDescriptor)
				;

			return await MapModelToPublic(q, true)
				.SingleOrDefaultAsync();
		}

		public async Task<TDocumentPublic> GetByKeyAsync(string Key)
		{
			var q = LimitedDocuments
				.Where(d => d.Ident == Key);
			return await MapModelToPublic(q, true).SingleOrDefaultAsync();
		}

		public async Task<TCategoryPublic> LoadContainerAsync(long Key)
		{
			var q = LimitedCategories
				.Where(i=>i.Id==Key)
				.EnabledScopedById(Key, ServiceInstanceDescriptor)
				;
			return await MapModelToPublic(q, true).SingleOrDefaultAsync();
		}

		public async Task<QueryResult<TDocumentPublic>> SearchAsync(string Key, Paging Paging)
		{
			var q = LimitedDocuments;
			if (!string.IsNullOrWhiteSpace(Key))
				q = q.Where(
					d=>
					d.Name.Contains(Key) || 
					d.Description.Contains(Key) || 
					d.Content.Contains(Key) || 
					d.SubTitle.Contains(Key) || 
					d.Remarks.Contains(Key)
					);

			return await q.ToQueryResultAsync(
				iq => MapModelToPublic(iq, false),
				r => r,
				docPageBuilder,
				Paging
				);
		}

		public async Task<QueryResult<TDocumentPublic>> ListItemsAsync(long? Container, Paging Paging)
		{

			var q = LimitedDocuments;
			if(Container.HasValue)
				q=q.Where(d => 
					d.ContainerId == Container 
					);
			return await q.ToQueryResultAsync(
				iq => MapModelToPublic(iq, false),
				r => r,
				docPageBuilder,
				Paging
				);
		}

		public async Task<QueryResult<TCategoryPublic>> ListChildContainersAsync(long? Key, Paging Paging)
		{
			var q = LimitedCategories.Where(d=>
				d.ContainerId == Key
				);
			return await q.ToQueryResultAsync(
				iq => MapModelToPublic(q,false),
				r => r,
				catPageBuilder,
				Paging
				);
		}
	}
}

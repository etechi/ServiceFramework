using SF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
using SF.Core.ServiceManagement;

namespace SF.Common.Documents
{
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
			return query.Select(EntityMapper.Map<TDocument, TDocumentPublic>());
		}
		protected virtual IContextQueryable<TCategoryPublic> MapModelToPublic(IContextQueryable<TCategory> query, bool detail)
		{
			return query.Select(EntityMapper.Map<TCategory, TCategoryPublic>());
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

		public async Task<TDocumentPublic> GetAsync(long Id)
		{
			var q = Documents.Value.AsQueryable()
				.EnabledScopedById(Id, ServiceInstanceDescriptor);

			return await MapModelToPublic(q, true)
				.SingleOrDefaultAsync();
		}

		public async Task<TDocumentPublic> GetByKeyAsync(string Key)
		{
			var q = LimitedDocuments.Where(d => d.Ident == Key);
			return await MapModelToPublic(q, true).SingleOrDefaultAsync();
		}

		public async Task<TCategoryPublic> LoadContainerAsync(long Key)
		{
			var q = LimitedCategories.EnabledScopedById(Key, ServiceInstanceDescriptor);
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
			var q = LimitedDocuments.Where(d => 
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

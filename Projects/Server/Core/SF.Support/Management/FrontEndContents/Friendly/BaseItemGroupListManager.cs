
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
{
	
	public abstract class BaseItemGroupListManager<TContent, TContentManager,TItem> :
	   IItemGroupListManager<TItem>
	   where TContentManager : IContentManager<TContent>
	   where TContent : Content
	   where TItem:LinkItemBase
	{
		protected abstract string ContentGroup { get; }
		public IContentManager<TContent> ContentManager { get; }
		public BaseItemGroupListManager(IContentManager<TContent> ContentManager)
		{
			this.ContentManager = ContentManager;
		}
		protected abstract TItem ContentToItem(ContentItem Content);
		protected abstract ContentItem ItemToContent(TItem Item);


		public async Task<ItemGroup<TItem>> GetAsync(ObjectKey<long> Id)
		{
			var re = await ContentManager.LoadForEdit(Id);
			if (re == null)
				throw new PublicArgumentException($"找不到{GetType().Comment()}");

			return new ItemGroup<TItem>
			{
				Id = re.Id,
				Help = re.Summary,
				Name = re.Name,
				Items = re.Items.Select(ContentToItem).ToArray()
			};
		}

		public async Task UpdateAsync(ItemGroup<TItem> Entity)
		{
			var re = await ContentManager.LoadForEdit(ObjectKey.From(Entity.Id));
			if (re == null)
				throw new PublicArgumentException($"找不到{GetType().Comment()}");

			re.Items = Entity.Items.Select(ItemToContent).ToArray();
			await ContentManager.UpdateAsync(re);
			
		}

		public async Task<ItemGroup<TItem>[]> ListAsync()
		{
			if (string.IsNullOrWhiteSpace(ContentGroup))
				throw new PublicArgumentException($"未设置{GetType().Comment()}");
			var re = await ContentManager.QueryAsync(new ContentQueryArgument
			{
				Category = ContentGroup
			}, Paging.Default);

			return re.Items.Select(it => new ItemGroup<TItem>
			{
				Id = it.Id,
				Name = it.Name,
				Help = it.Summary
			}).ToArray();
		}
	}
	
}
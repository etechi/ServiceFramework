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


using System.Linq;
using System.Threading.Tasks;

using SF.Sys.Entities;
using SF.Sys.Comments;
using SF.Sys;

namespace SF.Common.FrontEndContents.Friendly
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


		public async Task<ItemGroup<TItem>> GetAsync(ObjectKey<long> Id, string[] Fields = null)
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
				Category = ContentGroup,
				Paging=Paging.Default
			});

			return re.Items.Select(it => new ItemGroup<TItem>
			{
				Id = it.Id,
				Name = it.Name,
				Help = it.Summary
			}).ToArray();
		}
	}
	
}
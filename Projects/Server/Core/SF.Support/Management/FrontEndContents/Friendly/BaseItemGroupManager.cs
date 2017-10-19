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


using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
{
	
	public abstract class BaseItemGroupManager<TContent, TContentManager,TItem> :
	   IItemGroupManager<TItem>
	   where TContentManager : IContentManager<TContent>
	   where TContent : Content
	   where TItem:LinkItemBase
	{
		protected abstract long EntityId { get; }
		public IContentManager<TContent> ContentManager { get; }


		public BaseItemGroupManager(IContentManager<TContent> ContentManager)
		{
			this.ContentManager = ContentManager;
		}
		protected abstract TItem ContentToItem(ContentItem Content);
		protected abstract ContentItem ItemToContent(TItem Item);
		public Task<ItemGroup<TItem>> GetAsync(ObjectKey<long> key)
			=> LoadForEdit(key);

		public async Task<ItemGroup<TItem>> LoadForEdit(ObjectKey<long> key)
		{
			if (EntityId == 0)
				throw new PublicArgumentException($"未设置{GetType().Comment()}");
			var re = await ContentManager.LoadForEdit(ObjectKey.From(EntityId));
			if (re == null)
				throw new PublicArgumentException($"找不到{GetType().Comment()}");

			return new ItemGroup<TItem>
			{
				Items = re.Items.Select(ContentToItem).ToArray()
			};
		}
		public async Task UpdateAsync(ItemGroup<TItem> value)
		{
			if (EntityId == 0)
				throw new PublicArgumentException($"未设置{GetType().Comment()}");
			var re = await ContentManager.LoadForEdit(ObjectKey.From(EntityId));
			if (re == null)
				throw new PublicArgumentException($"找不到{GetType().Comment()}");

			re.Items = value.Items.Select(ItemToContent).ToArray();
			await ContentManager.UpdateAsync(re);
		}

	}
	
}
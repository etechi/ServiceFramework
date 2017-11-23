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

using SF.Management.FrontEndContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hygou.Setup
{
	public class ProductContentInitializer
	{
		public IContent AllProducts { get; private set; }
		public IContent AllRounds { get; private set; }
		public IContent[] CategoriesProducts { get; private set; }
		public IContent OpenWaitingRounds { get; private set; }
		public IContent OpenedRounds { get; private set; }

		public static async Task<ProductContentInitializer> Create(
			IContentManager<Content> ContentManager,
			ProductCategoryInitializer collections
			)
		{
			var prd_all = await ContentManager.ContentEnsure(
				"产品内容",
				"所有产品",
				"service",
				$"{{\"service\":\"Item\",\"method\":\"ListCategoryItems\",\"args\":{{\"CategoryId\":{collections.TypedCategoryRoot.Id}}}}}",
				$"/cat"
				);

			var round_all = await ContentManager.ContentEnsure(
				"夺宝内容",
				"所有夺宝轮次",
				"service",
				$"{{\"service\":\"Item\",\"method\":\"ListCategoryItems\",\"args\":{{\"CategoryId\":{collections.TypedCategoryRoot.Id}}}}}",
				$"/item"
				);

			var round_open_waiting = await ContentManager.ContentEnsure(
				"夺宝内容",
				"即将揭晓产品",
				"service",
				$"{{\"service\":\"Item\",\"method\":\"ListCategoryItems\",\"args\":{{\"CategoryId\":{collections.TypedCategoryRoot.Id}}}}}",
				$"/item"
				);
			var round_opened = await ContentManager.ContentEnsure(
				"夺宝内容",
				"最新揭晓产品",
				"service",
				$"{{\"service\":\"Item\",\"method\":\"ListCategoryItems\",\"args\":{{\"CategoryId\":{collections.TypedCategoryRoot.Id}}}}}",
				$"/item"
				);

			var cats = new List<IContent>();
			foreach(var t in collections.TypedCategorys)
				cats.Add(await ContentManager.ContentEnsure(
					"分类产品内容",
					t.Title,
					"service",
				$"{{\"service\":\"Item\",\"method\":\"ListCategoryItems\",\"args\":{{\"CategoryId\":{t.Id}}}}}",
					$"/cat/{t.Id}"
					));

			return new ProductContentInitializer
			{
				CategoriesProducts = cats.ToArray(),
				AllProducts = prd_all,
				OpenedRounds=round_opened,
				OpenWaitingRounds=round_open_waiting,
				AllRounds= round_all
			};
		}

	}
}

using SF.Management.FrontEndContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdMall.Setup
{
	public class ProductContentInitializer
	{
		public IContent AllProducts { get; private set; }
		public IContent AllRounds { get; private set; }
		public IContent[] CategoriesProducts { get; private set; }
		public IContent OpenWaitingRounds { get; private set; }
		public IContent OpenedRounds { get; private set; }

		public static async Task<ProductContentInitializer> Create(
			IDIScope scope,
			ProductCategoryInitializer collections
			)
		{
			var prd_all = await scope.ProductContentEnsure(
				"产品内容",
				"所有产品",
				"WebApi",
				$"{{\"controller\":\"Product\",\"action\":\"List\",\"id\":{collections.TypedCategoryRoot.Id}}}",
				$"/cat"
				);

			var round_all = await scope.ProductContentEnsure(
				"夺宝内容",
				"所有夺宝轮次",
				"WebApi",
				"{\"controller\":\"Round\",\"action\":\"List\"}",
				$"/open"
				);

			var round_open_waiting = await scope.ProductContentEnsure(
				"夺宝内容",
				"即将揭晓产品",
				"WebApi",
				"{\"controller\":\"Round\",\"action\":\"List\",\"State\":\"OpenWaiting\"}",
				$"/open"
				);
			var round_opened= await scope.ProductContentEnsure(
				"夺宝内容",
				"最新揭晓产品",
				"WebApi",
				"{\"controller\":\"Round\",\"action\":\"List\",\"State\":\"Opened\"}",
				$"/open"
				);

			var cats = new List<IContent>();
			foreach(var t in collections.TypedCategorys)
				cats.Add(await scope.ProductContentEnsure(
					"分类产品内容",
					t.Title,
					"WebApi",
					$"{{\"controller\":\"Product\",\"action\":\"List\",\"id\":{t.Id},\"_pm\":\"order\"}}",
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

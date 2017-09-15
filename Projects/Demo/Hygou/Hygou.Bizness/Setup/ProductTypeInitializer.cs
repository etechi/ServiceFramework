using SF.Biz.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hygou.Setup
{
	public class ProductTypeInitializer
	{
		public ProductTypeEditable[] Types { get; private set; }

		public static async Task<ProductTypeInitializer> Create(IProductTypeManager ProductTypeManager)
		{
			var type_names = new[]
			{
				"黄金",
				"汽车",
				 "手机",
				 "数码",
				 "话费充值",
			};
			var types = new List<ProductTypeEditable>();
			var idx = 1;
			foreach (var name in type_names)
			{
				types.Add(await ProductTypeManager.ProductTypeEnsure(
					new ProductTypeEditable
					{
						Name = name,
						Title = name,
						Icon = "yg-icons-" + idx + "-png",
						Image = "yg-icons-" + idx + "b-png"
					}
					));
				idx++;
			}
			return new ProductTypeInitializer
			{
				Types=types.ToArray()
			};
		}

	}
}

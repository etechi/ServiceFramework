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

using SF.Biz.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFShop.Setup
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

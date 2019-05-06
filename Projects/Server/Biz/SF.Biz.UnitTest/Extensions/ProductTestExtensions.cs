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

using System.Threading.Tasks;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.UnitTest;
using SF.Sys.Hosting;
using SF.Auth.IdentityServices;
using SF.Sys.Services;
using SF.Auth.IdentityServices.Externals;
using SF.Sys.Data;
using SF.Sys;
using SF.Biz.Products;
using System.Linq;
using SF.Sys.Entities;
using SF.Sys.Linq;

namespace SF.Biz.UnitTest
{
	
	public static class ProductTestExtensions
	{
        public static async Task<ProductTypeEditable> ProductTypeEnsure(
            this IServiceProvider sp,
            string name,
            string unit="unit",
            string title=null,
            string icon="icon",
            string image="image"
            )
        {
            title = title ?? name;
            var e = await sp.Resolve<IProductTypeManager>().ProductTypeEnsure(new ProductTypeEditable
            {
                Name = name,
                Unit = unit,
                Title = title,
                Icon=icon,
                Image=image
            });
            Assert.AreEqual(unit, e.Unit);
            Assert.AreEqual(name, e.Name);
            Assert.AreEqual(title,e.Title);
            Assert.AreEqual(icon, e.Icon);
            Assert.AreEqual(image, e.Image);

            var re =await sp.Resolve<IProductTypeManager>().GetAsync(ObjectKey.From(e.Id));

            Assert.AreEqual(unit, re.Unit);
            Assert.AreEqual(name, re.Name);
            Assert.AreEqual(title, re.Title);
            Assert.AreEqual(icon, re.Icon);
            Assert.AreEqual(image, re.Image);
            return re;
        }
        public static async Task<ProductInternal> ProductEnsure(
            this IServiceProvider sp,
            long seller,
            string type = null, 
            string name = null,
            decimal price = 100, 
            int priceunit = 1, 
            bool enabled = true,
            bool isVirtual = false
            )
        {
            name = name??"product-" + Strings.Numbers.Random(8);
            var marketPrice = price * 2;
            var image = "image1";
            var images = new[] { "image2", "image3" };
            var contents = new[] { "image4", "image5" };
            
            var t = await sp.ProductTypeEnsure(type ?? "TestProductType");
            var e = await sp.Resolve<IProductManager>().ProductEnsure(
                sellerId: seller,
                type: t.Id,
                name: name,
                marketPrice: marketPrice,
                price: price,
                priceUnit: 0,
                image: image,
                images: images,
                contentImages: contents,
                isVirtual: isVirtual,
                publishTime: null
                );
            Assert.AreEqual(name, e.Name);
            Assert.AreEqual(seller, e.OwnerUserId);
            Assert.AreEqual(marketPrice, e.MarketPrice);
            Assert.AreEqual(price, e.Price);
            //Assert.AreEqual(priceunit, re.CFPriceUnitExpect);
            Assert.AreEqual(image, e.Image);
            Assert.AreEqual(isVirtual, e.IsVirtual);
            Assert.AreEqual(images.Join(","), e.Content.Images.Select(i => i.Image).Join(","));
            Assert.AreEqual(contents.Join(","), e.Content.Descs.Select(i => i.Image).Join(","));

            var re = await sp.Resolve<IProductManager>().GetAsync(ObjectKey.From(e.Id));
            Assert.AreEqual(name, re.Name);
            Assert.AreEqual(marketPrice, re.MarketPrice);
            Assert.AreEqual(price, re.Price);
            //Assert.AreEqual(priceunit, re.CFPriceUnitExpect);
            Assert.AreEqual(image, re.Image);
            Assert.AreEqual(isVirtual, re.IsVirtual);
            //Assert.AreEqual(images, re.Content.Images.Select(i => i.Image).ToArray());
            //Assert.AreEqual(contents, re.Content.Descs.Select(i => i.Image).ToArray());
            return re;
        }
        public static async Task<ItemEditable> ProductItemEnsure(
            this IServiceProvider sp,
            long seller,
            long product)
        {

            var e = await sp.Resolve<IProductItemManager>().ItemEnsure(
                seller,
                product
                );
            var it = (await sp.Resolve<IItemService>().GetItems(new[] { e.Id })).First();
            Assert.AreEqual(seller, it.SellerId);
            Assert.AreEqual(product, it.ProductId);
            return e;
        }
        public static async Task ProductEnable(
            this IServiceProvider sp,
            long ProductId
            )
        {
            await sp.Resolve<IProductManager>().ProductEnable(ProductId);
        }

    }


}

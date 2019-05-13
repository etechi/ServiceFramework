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
using System.Collections.Generic;

namespace SF.Biz.UnitTest
{
	
	public static class ProductTestExtensions
	{
        public static async Task<ProductTypeEditable> ProductTypeEnsure(
            this IServiceProvider sp,
            string name=null,
            string unit="unit",
            string title=null,
            string icon="icon",
            string image="image"
            )
        {
            if (name == null)
                name = "type-" + Strings.NumberAndLowerChars.Random(8);

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

        public static async Task<ProductInternal[]> ProductsEnsure(
            this IServiceProvider sp,
            long? type,
            int Count,
            long seller,
            string name=null
            )
        {
            if(!type.HasValue)
                type = (await sp.ProductTypeEnsure()).Id;
            name = name ?? "product-" + Strings.Numbers.Random(8);
            var re = new List<ProductInternal>();
            for (var i = 0; i < Count; i++)
                re.Add(await sp.ProductEnsure(seller,name="product-"+i,typeId:type));
            return re.ToArray();
        }

        public static async Task<ProductInternal> ProductEnsure(
            this IServiceProvider sp,
            long seller,
            string name = null,
            decimal price = 100, 
            int priceunit = 1,
            long? typeId=null,
            EntityLogicState state = EntityLogicState.Enabled,
            bool isVirtual = false
            )
        {
            name = name??"product-" + Strings.Numbers.Random(8);
            var marketPrice = price * 2;
            var image = name+ "image1";
            var images = new[] { name+"image2", name + "image3" };
            var contents = new[] { name + "image4", name + "image5" };

            if (!typeId.HasValue)
                typeId = (await sp.ProductTypeEnsure()).Id;
            var e = await sp.Resolve<IProductManager>().ProductEnsure(
                sellerId: seller,
                type: typeId.Value,
                name: name,
                marketPrice: marketPrice,
                price: price,
                priceUnit: 0,
                image: image,
                images: images,
                contentImages: contents,
                isVirtual: isVirtual,
                publishTime: null,
                logicState:state
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
        public static async Task<ItemEditable[]> ProductItemsEnsure(
            this IServiceProvider sp,
            long seller,
            int Count,
            long? type = null,
            string name=null
            )
        {
            var ps = await sp.ProductsEnsure(type, Count, seller,name);
            var re = new List<ItemEditable>();
            foreach(var p in ps)
            {
                re.Add(await sp.ProductItemEnsure(seller, p.Id));
            }
            return re.ToArray();
        }
        public static async Task<ItemEditable> ProductItemEnsure(
            this IServiceProvider sp,
            long seller,
            long? product=null
            )
        {
            if (product == null)
                product = (await sp.ProductEnsure(seller)).Id;

            var e = await sp.Resolve<IProductItemManager>().ItemEnsure(
                seller,
                product.Value
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

﻿using SF.Biz.Products;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hygou.Setup
{
	public static class SampleImporter
	{
		static decimal parsePrice(string value)
        {
            value = value.Trim();
            if (string.IsNullOrEmpty(value))
                return 0;
            return decimal.Parse(new String(value.Where(c => char.IsDigit(c) || c == '.').ToArray()));
        }

        static string format(string s)
        {
            if (s == null) return null;
            return StaticRes.Product+"-" + s.Replace("/", "-").Replace(".", "-");
        }

        public static async Task ImportSamples(
			IProductManager productManager,
			IItemManager itemManager,
			IProductTypeManager ProductTypeManager,
			SF.Core.Hosting.IFilePathResolver pathResolver,
			long SellerId
			)
		{
            var list = System.IO.File.ReadAllLines(pathResolver.Resolve("root://StaticResources/产品数据/产品清单.csv"),Encoding.GetEncoding("GBK")).Skip(1).Select(l =>
                    {
                        var ss = l.Split(',');
                        return new
                        {
                            id = ss[0].Trim(),
                            name = ss[1].Trim(),
                            marketprice = parsePrice(ss[2]),
                            price = parsePrice(ss[3]),
                            unit = parsePrice(ss[4]),
                            time = DateTime.Parse(ss[5]),
                            cats = ss[6].Split(';').Select(s=>s.Trim()).ToArray(),
                            isVirtual=ss[7]=="是"
                        };
                    }
            );
			var typeMap = (await ProductTypeManager.QueryAllAsync()).ToDictionary(t => t.Name, t => t.Id);

            foreach (var item in list)
			{
                var images = System.IO.Directory.GetFiles(
					pathResolver.Resolve($"root://StaticResources/产品数据/产品图片/{item.id}/imgs"),
                    "*.*"
                    ).Select(f => new
                    {
                        file = f,
                        name = System.IO.Path.GetFileNameWithoutExtension(f),
                        ext= System.IO.Path.GetExtension(f).Substring(1)
                    }).Where(f=>f.name[0]!='$' && (f.ext=="jpg" || f.ext=="png")).OrderBy(f => f.name).ToArray();

                var intros= System.IO.Directory.GetFiles(
					pathResolver.Resolve($"root://StaticResources/产品数据/产品图片/{item.id}/intro"),
                    "*.*"
                    ).Select(f => new
                    {
                        file = f,
                        name = System.IO.Path.GetFileNameWithoutExtension(f),
                        ext = System.IO.Path.GetExtension(f).Substring(1)
                    }).Where(f => f.name[0] != '$' && (f.ext == "jpg" || f.ext == "png")).OrderBy(f => f.name).ToArray();

                if (images.Length == 0)
                    continue;
                var p = await productManager.ProductEnsure(
					SellerId,
					typeMap[item.cats[0]],
					item.name,
					item.marketprice==0?item.price:item.marketprice,
					item.price,
					Math.Max((int)item.unit,1),
					format(item.id+"-imgs-"+images[0].name+"-"+ images[0].ext),
                    images.Select(f=>format(item.id + "-imgs-" + f.name + "-"+f.ext)).ToArray(),
                    intros.Select(f => format(item.id + "-intro-" + f.name + "-"+f.ext)).ToArray(),
                    item.isVirtual
                    );
				await productManager.ProductEnable(p.Id);
				await itemManager.ItemEnsure(p.OwnerUserId, p.Id);
			}
		}
	}
}

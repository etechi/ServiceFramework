using SF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Services.FrontEndContents.Runtime
{


	class Site
	{
		public string Name { get; private set; }
		public Dictionary<string,Page> Pages { get; private set; }
		
		static void PageIncludeResolve(Dictionary<string,Page> pages, Page page,HashSet<Page> hash, SiteLoadContext ctx, List<Page> result)
		{
			if (!hash.Add(page))
				return;
			result.Add(page);
			if(page.Includes!=null)
				foreach(var ins in page.Includes)
				{
					Page cli;
					if (!pages.TryGetValue(ins, out cli))
					{
						ctx.Messages.Add($"找不到页面{page.Id}的引用页面{ins}");
						continue;
					}
					PageIncludeResolve(pages, cli, hash, ctx, result);
				}
		}

		public static Site Create(
			string siteConfig,
			IUIConfigLoader configLoader
			)
		{
			var siteModel = Json.Parse<SiteConfigModels.SiteModel>(siteConfig);

			var loadContext = new SiteLoadContext();

			var pages = siteModel.pages
				.Where(p=>!(p.disabled ??false))
				.Select(pm => Page.Create(siteModel,pm, loadContext))
				.ToDictionary(p => p.Id);


			var new_pages = new Dictionary<string, Page>();
			foreach(var page in pages.Values)
			{
				var result = new List<Page>();
				var hash = new HashSet<Page>();
				PageIncludeResolve(pages, page, hash, loadContext, result);
				var blocks = new Dictionary<string, Block>();
				foreach(var p in result)
					foreach(var b in p.Blocks)
					{
						if (blocks.ContainsKey(b.Id)) continue;
						blocks[b.Id] = b;
					}
				new_pages[page.Id] = Page.Create(page, blocks.Values.ToArray());
			}

			return new Site
			{
				Name=siteModel.name,
				Pages = new_pages
			};
		}
	}
	
}

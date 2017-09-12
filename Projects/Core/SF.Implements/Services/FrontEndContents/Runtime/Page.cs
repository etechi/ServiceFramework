using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Services.FrontEndContents.Runtime
{
	class Page
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public Block[] Blocks { get; private set; }
		public string[] Includes { get; private set; }
		public static Page Create(
			Page page,
			Block[] Blocks
			)
		{
			return new Page
			{
				Id = page.Id,
				Name = page.Name,
				Blocks = Blocks,
				Includes = page.Includes
			};
		}

		public static Page Create(
			SiteConfigModels.SiteModel siteModel,
			SiteConfigModels.PageModel model, 
			SiteLoadContext loadContext
			)
		{
            var blocks = model.blocks
                ?.Where(b => !(b.disabled ?? false))
                .Select(bm => Block.Create(siteModel, model, bm, loadContext))
                .ToArray() ?? Array.Empty<Block>();

			return new Page
			{
				Id = model.ident,
				Name = model.name,
				Blocks = blocks,
				Includes=model.includes
			};
		}
	}
	
}

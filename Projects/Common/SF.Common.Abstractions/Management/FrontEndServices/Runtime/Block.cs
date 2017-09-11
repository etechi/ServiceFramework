using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.DI;

namespace ServiceProtocol.Biz.UIManager.Runtime
{
	
	
	class Block
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public BlockContent[] Contents { get; private set; }

		public static Block Create(
			SiteConfigModels.SiteModel siteModel,
			SiteConfigModels.PageModel pageModel,
			SiteConfigModels.BlockModel model,
			SiteLoadContext loadContext
			)
		{
			var contents = model.contents
				.Where(c => !(c.disabled ?? false))
				.Select(cm => BlockContent.Create(siteModel, pageModel, model, cm, loadContext))
				.Where(bc => bc != null)
				.ToArray();

			return new Block
			{
				Id = model.ident,
				Name = model.name,
				Contents = contents
			};
		}
	}
	
}

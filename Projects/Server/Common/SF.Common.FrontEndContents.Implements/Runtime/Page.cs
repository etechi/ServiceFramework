﻿#region Apache License Version 2.0
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Common.FrontEndContents.Runtime
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

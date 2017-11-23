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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Common.FrontEndContents.Runtime
{


	class BlockContent
	{
		public long ContentId { get; private set; }
		public string ContentConfig { get; private set; }
		public string Image { get; private set; }
		public string Icon { get; private set; }
		public string FontIcon { get; private set; }
		public string Uri { get; private set; }
		public string UriTarget { get; private set; }
		public string Title1 { get; private set; }
		public string Title2 { get; private set; }
		public string Title3 { get; private set; }
		public string Summary { get; private set; }
		public string RenderProvider { get; private set; }
		public string RenderView { get; private set; }
		public string RenderViewConfig { get; private set; }
		public bool Disabled { get { return false; } }

		public static BlockContent Create(
			SiteConfigModels.SiteModel siteModel,
			SiteConfigModels.PageModel pageModel, 
			SiteConfigModels.BlockModel blockModel, 
			SiteConfigModels.BlockContentModel model, 
			SiteLoadContext loadContext
			)
		{
		
			return new BlockContent
			{
				ContentId=model.content ?? 0,
				ContentConfig=model.contentConfig,
				Image = model.image,
				Icon = model.icon,
				FontIcon = model.fontIcon,
				Uri = model.uri,
				Title1 = model.title1,
				Title2 = model.title2,
				Title3 = model.title3,
				Summary = model.summary,
				RenderProvider = model.render,
				RenderView = model.view,
				RenderViewConfig=model.viewConfig
			};
		}
	}

}

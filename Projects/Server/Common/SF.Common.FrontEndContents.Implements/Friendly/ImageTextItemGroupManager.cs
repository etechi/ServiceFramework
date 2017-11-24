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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.FrontEndContents.Friendly
{
	
	public abstract class ImageTextItemGroupManager<TContent,TContentManager> :
		BaseItemGroupManager<TContent, TContentManager, ImageTextItem>,
		IImageTextItemGroupManager
		where TContentManager:IContentManager<TContent>
		where TContent:Content
	{
		public ImageTextItemGroupManager(IContentManager<TContent> ContentManager) : base(ContentManager)
		{
		}


		protected override ImageTextItem ContentToItem(ContentItem i)
			=> new ImageTextItem
			{
				Link = i.Uri,
				LinkTarget = LinkTargetConvert.ToFriendly(i.UriTarget),
				Text1 = i.Title1,
				Text2 = i.Title2,
				Image = i.Image
			};
		protected override ContentItem ItemToContent(ImageTextItem it)
			=>
			new ContentItem
			{
				Title1 = it.Text1,
				Title2 = it.Text2,
				Image = it.Image,
				Uri = it.Link,
				UriTarget = LinkTargetConvert.FromFriendly(it.LinkTarget),
			};
	}
}
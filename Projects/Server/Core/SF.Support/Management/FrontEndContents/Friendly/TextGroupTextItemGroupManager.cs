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

namespace SF.Management.FrontEndContents.Friendly
{
	
	public abstract class TextGroupTextItemGroupManager<TContent,TContentManager> :
		BaseItemGroupManager<TContent, TContentManager, TextGroupItem<TextItem>>,
		ITextGroupTextItemGroupManager
		where TContentManager:IContentManager<TContent>
		where TContent:Content
	{
		public TextGroupTextItemGroupManager(IContentManager<TContent> ContentManager) : base(ContentManager)
		{
		}

		protected override TextGroupItem<TextItem> ContentToItem(ContentItem i)
			=> new Friendly.TextGroupItem<Friendly.TextItem>
			{
				Text = i.Title1,
				Items = i.Items?.Select(ii => new Friendly.TextItem
				{
					Link = ii.Uri,
					LinkTarget = LinkTargetConvert.ToFriendly(ii.UriTarget),
					Text1 = ii.Title1,
					Text2 = ii.Title2
				})?.ToArray()
			};
		protected override ContentItem ItemToContent(TextGroupItem<TextItem> it)
			=>
			new ContentItem
			{
				Title1 = it.Text,
				Uri = it.Link,
				UriTarget = LinkTargetConvert.FromFriendly(it.LinkTarget),
				Items = it.Items?.Select(iit =>
				new ContentItem
				{
					Title1 = iit.Text1,
					Title2 = iit.Text2,
					Uri = iit.Link,
					UriTarget = LinkTargetConvert.FromFriendly(iit.LinkTarget)
				})?.ToArray()
			};
	}
}
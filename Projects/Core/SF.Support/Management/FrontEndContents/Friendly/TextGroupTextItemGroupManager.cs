
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
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
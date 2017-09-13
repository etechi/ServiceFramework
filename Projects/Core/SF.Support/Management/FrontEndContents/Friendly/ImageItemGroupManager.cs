
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
{
	
	public abstract class ImageItemGroupManager<TContent,TContentManager> :
		BaseItemGroupManager<TContent, TContentManager, ImageItem>,
		IImageItemGroupManager
		where TContentManager:IContentManager<TContent>
		where TContent:Content
	{
		public ImageItemGroupManager(IContentManager<TContent> ContentManager) : base(ContentManager)
		{
		}

		protected override ImageItem ContentToItem(ContentItem i)
			=> new ImageItem
			{
				Link = i.Uri,
				LinkTarget = LinkTargetConvert.ToFriendly(i.UriTarget),
				Image = i.Image
			};
		protected override ContentItem ItemToContent(ImageItem it)
			=>
			new ContentItem
			{
				Image = it.Image,
				Uri = it.Link,
				UriTarget = LinkTargetConvert.FromFriendly(it.LinkTarget),
			};
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
{
	
	public abstract class BaseItemGroupListManager<TContent, TContentManager,TItem> :
	   IItemGroupListManager<TItem>
	   where TContentManager : IContentManager<TContent>
	   where TContent : Content
	   where TItem:LinkItemBase
	{
		protected abstract string ContentGroup { get; }
		public IContentManager<TContent> ContentManager { get; }
		public BaseItemGroupListManager(IContentManager<TContent> ContentManager)
		{
			this.ContentManager = ContentManager;
		}
		protected abstract TItem ContentToItem(ContentItem Content);
		protected abstract ContentItem ItemToContent(TItem Item);

		
	}
	
}
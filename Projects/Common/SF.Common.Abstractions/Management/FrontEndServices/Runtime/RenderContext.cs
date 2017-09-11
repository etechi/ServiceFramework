using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.DI;

namespace ServiceProtocol.Biz.UIManager.Runtime
{
	
	class BlockContentRenderContext : IBlockContentRenderContext
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public string Image { get; set; }
		public string Icon { get; set; }
		public string FontIcon { get; set; }
		public string Uri { get; set; }
		public string UriTarget { get; set; }
		public string Title1 { get; set; }
		public string Title2 { get; set; }
		public string Title3 { get; set; }
		public string Summary { get; set; }
		public string ProviderType { get; set; }
		public string ProviderConfig { get; set; }
		public bool Disabled { get { return false; } }
		public ContentItem[] Items { get; set; }
		public IContent Content { get { return this; } }
		public object Data { get; set; }
		public string ContentConfig { get; set; }
		public string RenderProvider { get; set; }
		public string RenderView { get; set; }
		public string RenderConfig { get; set; }
	}
	class BlockRenderContext : IBlockRenderContext
	{
		public string Id { get; set; }
		internal IBlockContentRenderContext[] BlockContentContexts { get; set; }
		public IEnumerable<IBlockContentRenderContext> BlockContents
		{
			get { return BlockContentContexts; }
		}
	}
	class PageRenderContext : 
		IPageRenderContext
	{
		Dictionary<string, IBlockRenderContext> blocks { get; }
		public string Id { get; }
		public IEnumerable<IBlockRenderContext> Blocks { get { return blocks.Values; } }
		public PageRenderContext(string id, Dictionary<string, IBlockRenderContext> blocks)
		{
			this.Id = id;
			this.blocks = blocks;
		}
		public IBlockRenderContext GetBlockRenderContext(string block)
		{
			IBlockRenderContext brc;
			if (!blocks.TryGetValue(block, out brc))
				return null;
			return brc;
		}
	}

	class SiteRenderContext : ISiteRenderContext
	{
		public IPageRenderContext[] Pages { get; }
		public SiteRenderContext(IPageRenderContext[] Pages)
		{
			this.Pages = Pages;
		}
	}


}

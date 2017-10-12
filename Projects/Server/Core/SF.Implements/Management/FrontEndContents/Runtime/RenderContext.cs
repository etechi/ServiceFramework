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
namespace SF.Management.FrontEndContents.Runtime
{

	class BlockContentRenderContext : IBlockContentRenderContext
	{
		public long Id { get; set; }
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

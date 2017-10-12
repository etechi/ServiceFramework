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

using SF.Core.Logging;
using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Management.FrontEndContents.Runtime
{
	class RenderContextBuilder 
	{
		public ILogger Logger { get; }
		public NamedServiceResolver<IDataProvider> DataProviderResolver { get; }
		public RenderContextBuilder(ILogger Logger, NamedServiceResolver<IDataProvider> DataProviderResolver)
		{
			this.Logger = Logger;
			this.DataProviderResolver = DataProviderResolver;

		}
		void Log(string message,Exception e=null)
		{
			Logger.Write( LogLevel.Error,EventId.None, e, message);
		}

		async Task<IBlockContentRenderContext> CreateBlockContentRenderContext(
			Site site,
			Page page,
			Block block,
			string render,
			string view,
			string viewConfig,
			BlockContent content,
			ISiteRenderEngine Engine,
			Lazy<IContentLoader> ContentLoader,
			bool LoadData
			)
		{
			object data = null;
			IContent ctn = null;
			if (content.ContentId != 0 && ContentLoader!=null)
			{
				ctn = await Engine.GetContent(content.ContentId,ContentLoader);
				if (ctn == null)
					return null;
				if (ctn.Disabled)
					return null;
				if (!string.IsNullOrEmpty(ctn.ProviderType) && LoadData)
				{
					IDataProvider dataProvider = null;
					try
					{
						dataProvider = DataProviderResolver(ctn.ProviderType);
						if(dataProvider==null)
						{
							Log( $"UI管理器找不到数据提供者: 提供者类型：{ctn.ProviderType} 站点:{site.Name} 页面:{page.Id} UI块:{block.Id} 内容:{content.ContentId} 渲染器:{render} 视图:{view} ");
						}
					}catch(Exception e)
					{
						Log($"UI管理器查找数据提供者时发生异常: 提供者类型：{ctn.ProviderType} 站点:{site.Name} 页面:{page.Id} UI块:{block.Id} 内容:{content.ContentId} 渲染器:{render} 视图:{view} ", e);
					}
					if(dataProvider!=null)
						try
						{
							data = await dataProvider.Load(ctn, content.ContentConfig);
						}
						catch(Exception e) {
							Log($"UI管理器载入数据时发生异常: 提供者类型：{ctn.ProviderType} 站点:{site.Name} 页面:{page.Id} UI块:{block.Id} 内容:{content.ContentId} 渲染器:{render} 视图:{view} 内容配置:{ctn.ProviderConfig} UI块配置:{content.ContentConfig}",e);
						}
				}
			}
			return new BlockContentRenderContext
			{
				Id = content.ContentId,
				Name = ctn?.Name,
				Category = ctn?.Category,
				Image = content.Image ?? ctn?.Image,
				Icon = content.Icon ?? ctn?.Icon,
				FontIcon = content.FontIcon ?? ctn?.FontIcon,
				Uri = content.Uri ?? ctn?.Uri,
				UriTarget = content.UriTarget ?? ctn?.UriTarget,
				Title1 = content.Title1 ?? ctn?.Title1,
				Title2 = content.Title2 ?? ctn?.Title2,
				Title3 = content.Title3 ?? ctn?.Title3,
				Summary = content.Summary ?? ctn?.Summary,
				ProviderType = ctn?.ProviderType,
				ProviderConfig = ctn?.ProviderConfig,
				ContentConfig=content.ContentConfig,
				Items = ctn?.Items,
				Data = data,
				RenderProvider =render,
				RenderView = view,
				RenderConfig=viewConfig
			};
		}
		 async Task<IBlockRenderContext> CreateBlockRenderContext(
			Site Site,
			Page Page,
			Block block,
			ISiteRenderEngine Engine,
			Lazy<IContentLoader> ContentLoader,
			bool LoadData
			)
		{
			var ctns = new List<IBlockContentRenderContext>();
			foreach (var c in block.Contents)
			{
				var bc = await CreateBlockContentRenderContext(
					Site,
					Page,
					block,
					c.RenderProvider,
					c.RenderView,
					c.RenderViewConfig,
					c,
					Engine,
					ContentLoader,
					LoadData
					);
				if (bc == null)
					continue;
				ctns.Add(bc);
			}
			if (ctns.Count == 0)
				return null;

			return new BlockRenderContext
			{
				Id = block.Id,
				BlockContentContexts = ctns.ToArray()
			};
		}
		public async Task<IPageRenderContext> BuildPageRenderContext(
			Site Site,
			Page Page,
			IDictionary<string, object> args,
			ISiteRenderEngine Engine,
			Lazy<IContentLoader> ContentLoader,
			bool LoadData =true
			)
		{
			var dic = new Dictionary<string, IBlockRenderContext>();
			foreach (var b in Page.Blocks)
			{
				var re = await CreateBlockRenderContext(
					Site,
					Page,
					b, 
					Engine,
					ContentLoader, 
					LoadData
					);
				if (re == null)
					continue;
				dic[re.Id] = re;
			}

			return new PageRenderContext(Page.Id, dic); ;
		}
		public  async Task<ISiteRenderContext> BuildSiteRenderContext(
			Site Site,
			ISiteRenderEngine Engine,
			Lazy<IContentLoader> ContentLoader
			)
		{
			var pages = new List<IPageRenderContext>();
			foreach (var p in Site.Pages)
				pages.Add(await BuildPageRenderContext(
					Site,
					p.Value, 
					null, 
					Engine,
					ContentLoader, 
					false
					));
			return new SiteRenderContext(pages.ToArray());
		}
	}
	
}

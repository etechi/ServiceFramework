using SF.Core.ServiceManagement;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndServices
{
    [Comment(Name = "界面管理器数据源")]
    public interface IDataProvider
	{
		Task<object> Load(IContent content,string blockContentConfig);
	}
    [Comment(Name = "界面管理器渲染器")]
    public interface IRenderProvider
	{
		void Render(object rawRenderContext,string view,string config, IContent content,object data);
	}

    //[RequireServiceType(typeof(IDataProvider), "界面管理器数据源")]
    //[RequireServiceType(typeof(IRenderProvider), "界面管理器渲染器")]
    public class ServiceTypeDeclare
    {

    }


	public interface IRenderConfig
	{
		string RenderProvider { get; }
		string RenderView { get; }
		string RenderConfig { get; }
	}
	public interface IBlockContentRenderContext : IContent, IRenderConfig
	{
		object Data { get; }
		string ContentConfig { get; }

	}
	public interface IBlockRenderContext
	{
		string Id { get; }
		IEnumerable<IBlockContentRenderContext> BlockContents { get; }
	}
	public interface IPageRenderContext
	{
		string Id { get; }
		IEnumerable<IBlockRenderContext> Blocks { get; }
		IBlockRenderContext GetBlockRenderContext(string block);
	}
	public interface ISiteRenderContext
	{
		IPageRenderContext[] Pages { get; }
	}
	public interface ISiteResolver
	{
		Task<long> FindTemplateId(string site);
	}

	public interface ISiteConfigLoader
	{
		Task<string> LoadConfig(long templateId);
	}

	public interface IContentLoader
	{
		Task<IContent> LoadContent(long contentId);
	}

	public interface IUIConfigLoader
	{
		ISiteConfigLoader SiteConfigLoader { get; }
		ISiteResolver SiteResolver { get; }
		IContentLoader ContentLoader { get; }
	}

    public interface ISiteRenderEngine
    {
		Task<IContent> GetContent(
			long contentId, 
			IContentLoader Loader
			);
		Task<IPageRenderContext> CreatePageContext(
			string site,
			string page,
			IDictionary<string, object> args,
			Func<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			);
		Task<IPageRenderContext> CreatePageContext(
			int templateId,
			string page,
			IDictionary<string, object> args,
			Func<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			);
		Task<ISiteRenderContext> CreateSiteContext(
			string site,
			Func<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			);
		Task<ISiteRenderContext> CreateSiteContext(
			int templateId,
			Func<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			);

		void NotifySiteBindChanged(string site);
		void NotifySiteTemplateChanged(long templateId);
		void NotifyContentChanged(long contentId);
	}
	public interface IRenderContextCreator
	{
		Task<IPageRenderContext> CreatePageContext(
			string site,
			string page,
			IDictionary<string, object> args,
			IServiceScope scope
			);
		Task<IPageRenderContext> CreatePageContext(
			 int templateId,
			 string page,
			 IDictionary<string, object> args,
			 IServiceScope scope
			 );
		Task<ISiteRenderContext> CreateSiteContext(
			string site,
			IServiceScope scope
			);
		Task<ISiteRenderContext> CreateSiteContext(
			 long templateId,
			 IServiceScope scope
			 );
	}
}

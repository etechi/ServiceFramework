using SF.Core.ServiceManagement;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.FrontEndContents
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
		Lazy<ISiteConfigLoader> SiteConfigLoader { get; }
		Lazy<ISiteResolver> SiteResolver { get; }
		Lazy<IContentLoader> ContentLoader { get; }
	}

    public interface ISiteRenderEngine
    {
		Task<IContent> GetContent(
			long contentId, 
			Lazy<IContentLoader> Loader
			);
		Task<IPageRenderContext> CreatePageContext(
			string site,
			string page,
			IDictionary<string, object> args,
			NamedServiceResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			);
		Task<IPageRenderContext> CreatePageContext(
			long templateId,
			string page,
			IDictionary<string, object> args,
			NamedServiceResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			);
		Task<ISiteRenderContext> CreateSiteContext(
			string site,
			NamedServiceResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			);
		Task<ISiteRenderContext> CreateSiteContext(
			long templateId,
			NamedServiceResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			);

		//void NotifySiteBindChanged(string site);
		//void NotifySiteTemplateChanged(long templateId);
		//void NotifyContentChanged(long contentId);
	}
	public interface IRenderContextCreator
	{
		Task<IPageRenderContext> CreatePageContext(
			string site,
			string page,
			IDictionary<string, object> args
			);
		Task<IPageRenderContext> CreatePageContext(
			 long templateId,
			 string page,
			 IDictionary<string, object> args
			 );
		Task<ISiteRenderContext> CreateSiteContext(
			string site
			);
		Task<ISiteRenderContext> CreateSiteContext(
			 long templateId
			 );
	}
}

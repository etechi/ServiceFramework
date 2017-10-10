using SF.Common.Documents;
using SF.Common.Documents.Management;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;
using SF.Management.FrontEndContents;
using SF.Management.FrontEndContents.Friendly;
using SF.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hygou.Setup
{
	public static class DocInitializer
	{

		public static async Task<Tuple<long,long>> PCHelpEnsure(
			IServiceInstanceManager sim,
			long? ScopeId,
			IContentManager ContentManger,
			IFilePathResolver FilePathResolver,
			IDocumentManager DocManager,
			IDocumentCategoryManager CatManager
			)
        {
            var re=await DocManager.DocEnsureFromFiles(
                CatManager,
                //"pc-help",
                null,
				FilePathResolver.Resolve($"root://StaticResources/帮助文档/PC文档")
                );
			await sim.UpdateSetting<HygouSetting>(
			   ScopeId,
			   s =>
			   {
				   s.PCHelpCenterDefaultDocId = re[0].Children[0].Id;
			   });
			
            var tail_doc_list = await ContentManger.ContentEnsure(
                "PC页面尾部文档列表",
                "PC页面尾部文档列表",
                null,
                re.Select(cat=>
                    new ContentItem{
                        Title1 =cat.Name,
                        Items = cat.Children.Select(doc=>
                            new ContentItem{Title1=doc.Name, Uri=$"/help/doc/{doc.Id}"}
                            ).ToArray()
                        }
                    ).Take(4).ToArray()
                    );
            var tail_link_list = await ContentManger.ContentEnsure(
                "PC页面尾部链接列表",
                "PC页面尾部链接列表",
                null,
                re.Last().Children.Select(doc=>
                    new ContentItem { Title1 = doc.Name, Uri = $"/help/doc/{doc.Id}"}
                ).ToArray()
            );

			await sim.UpdateSetting<FriendlyContentSetting>(
			   ScopeId,
			   s =>
			   {
				   s.PCHomeTailMenuId = tail_doc_list.Id;
			   });
			
            return Tuple.Create(tail_doc_list.Id, tail_link_list.Id);
        }
        public static async Task MobileHelpEnsure(
			IFilePathResolver FilePathResolver,
			IDocumentManager DocManager,
			IDocumentCategoryManager CatManager
			)
        {
            await DocManager.DocEnsureFromFiles(
			   CatManager,
			   null,
 				FilePathResolver.Resolve($"root://StaticResources/帮助文档/手机文档"),
			  null
			   );
        }

        public static async Task<Tuple<long,long>> DocEnsure(
			IServiceProvider ServiceProvider,
			IServiceInstanceManager sim,
			long? ScopeId
			)
		{
			var pcSys = await (sim.NewDocumentService("pc-sys")
				.WithIdent("pc-sys")
				.WithDisplay("PC系统文档", "PC系统文档，如关于我们等")
				.Enabled()
				).Ensure(ServiceProvider, ScopeId);

			var pcHelp = await (sim.NewDocumentService("pc-help")
				.WithIdent("pc-help")
				.WithDisplay("PC帮助文档")
				.Enabled()
				).Ensure(ServiceProvider, ScopeId);

			var mSys = await (sim.NewDocumentService("m-sys")
				.WithIdent("m-sys")
				.WithDisplay("移动端系统文档","移动端系统文档，如关于我们等")
				.Enabled()
				).Ensure(ServiceProvider, ScopeId);

			var mHelp = await (sim.NewDocumentService("m-help")
				.WithIdent("m-help")
				.WithDisplay("移动端帮助文档")
				.Enabled()
				).Ensure(ServiceProvider, ScopeId);

			
			await ServiceProvider.Invoke(
				((IFilePathResolver frr, IDocumentManager dm, IDocumentCategoryManager cm) arg) => MobileHelpEnsure(arg.frr, arg.dm, arg.cm),
				mHelp
				);

			var re=await ServiceProvider.Invoke(
				((IContentManager ContentManager, 
				IFilePathResolver frr, 
				IDocumentManager dm, 
				IDocumentCategoryManager cm
				) arg) => PCHelpEnsure(sim,ScopeId,arg.ContentManager, arg.frr, arg.dm, arg.cm),
				pcHelp
				);
			//await scope.MobileDocEnsureNew();
			return re;
		}
	}
}

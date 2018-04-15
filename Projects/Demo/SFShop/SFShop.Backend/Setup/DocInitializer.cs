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

using SF.Common.Documents;
using SF.Common.Documents.Management;
using SF.Common.FrontEndContents;
using SF.Sys.Hosting;
using SF.Sys.Services.Management;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Settings;
using SF.Common.FrontEndContents.Friendly;

namespace SFShop.Setup
{
	public static class DocInitializer
	{

		public static async Task<Tuple<long, long>> PCHelpEnsure(
			IServiceInstanceManager sim,
			long? ScopeId,
			IContentManager ContentManger,
			IFilePathResolver FilePathResolver,
			IDocumentManager DocManager,
			IDocumentCategoryManager CatManager
			)
		{
			//特殊文档
			await DocManager.DocEnsureFromFiles(
				CatManager,
				//"pc-help",
				null,
				FilePathResolver.Resolve($"root://StaticResources/帮助文档/特殊文档"),
				"sys"
				);

			var re = await DocManager.DocEnsureFromFiles(
				CatManager,
				//"pc-help",
				null,
				FilePathResolver.Resolve($"root://StaticResources/帮助文档/PC文档")
				);
			await sim.UpdateSetting<AppSetting>(
			   ScopeId,
			   s =>
			   {
				   s.PCHelpCenterDefaultDocId = re[0].Children[0].Id;
			   });

			var tail_doc_list = await ContentManger.ContentEnsure(
				"PC页面尾部文档列表",
				"PC页面尾部文档列表",
				null,
				re.Select(cat =>
					new ContentItem
					{
						Title1 = cat.Name,
						Items = cat.Children.Select(doc =>
							new ContentItem { Title1 = doc.Name, Uri = $"/help/doc/{doc.Id}" }
							).ToArray()
					}
					).Take(4).ToArray()
					);
			var tail_link_list = await ContentManger.ContentEnsure(
				"PC页面尾部链接列表",
				"PC页面尾部链接列表",
				null,
				re.Last().Children.Select(doc =>
					new ContentItem { Title1 = doc.Name, Uri = $"/help/doc/{doc.Id}" }
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


		public static async Task DocServiceEnsure(
			IServiceProvider ServiceProvider,
			IServiceInstanceManager sim,
			long? ScopeId
			)
		{

			//await(sim.NewDocumentService("PC系统文档", "pc-sys")
			//	.WithDisplay("PC系统文档", "PC系统文档，如关于我们等")
			//	.Enabled()
			//	).Ensure(ServiceProvider, ScopeId);

			//await (sim.NewDocumentService("PC帮助文档", "pc-help")
			//	.WithDisplay("PC帮助文档")
			//	.Enabled()
			//	).Ensure(ServiceProvider, ScopeId);

			//await(sim.NewDocumentService("移动端系统文档", "m-sys")
			//	.WithDisplay("移动端系统文档", "移动端系统文档，如关于我们等")
			//	.Enabled()
			//	).Ensure(ServiceProvider, ScopeId);

			//await (sim.NewDocumentService("移动端帮助文档", "m-help")
			//	.WithDisplay("移动端帮助文档")
			//	.Enabled()
			//	).Ensure(ServiceProvider, ScopeId);

		}

		public static async Task<Tuple<long, long>> DocDataEnsure(
			IServiceProvider ServiceProvider,
			IServiceInstanceManager sim,
			long? ScopeId
			)
		{

			//var mHelp = await sim.GetService<IDocumentService>();

			//await ServiceProvider.WithServices(
			//	((IFilePathResolver frr, IDocumentManager dm, IDocumentCategoryManager cm) arg) => 
			//		MobileHelpEnsure(arg.frr, arg.dm, arg.cm),
			//	mHelp.Id
			//	);

			//var pcHelp = await sim.GetService<IDocumentService>("pc-help", ScopeId);

			var re = await ServiceProvider.WithServices(
				((IContentManager ContentManager,
				IFilePathResolver frr,
				IDocumentManager dm,
				IDocumentCategoryManager cm
				) arg) =>
					PCHelpEnsure(sim, ScopeId, arg.ContentManager, arg.frr, arg.dm, arg.cm)
				//pcHelp.Id
				);
			//await scope.MobileDocEnsureNew();
			return re;
		}
	}
}

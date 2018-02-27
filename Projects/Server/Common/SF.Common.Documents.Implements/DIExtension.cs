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

using SF.Common.Documents.Management;
using SF.Common.Documents;
using SF.Sys.Services.Management;
using SF.Sys.Entities.AutoTest;
using SF.Sys.Entities.AutoEntityProvider;
using SF.Sys.Entities;

namespace SF.Sys.Services
{
	public static class DocumentDIExtension
		
	{
		public static IServiceCollection AddDocumentServices(this IServiceCollection sc,string TablePrefix=null)
		{
			//文章
			sc.EntityServices(
				"Document",
				"文档管理",
				d => d.Add<IDocumentCategoryManager, DocumentCategoryManager>("DocumentCategory","文档分类",typeof(Category))
					.Add<IDocumentManager, DocumentManager>("Document", "文档", typeof(Document))
					.Add<IDocumentScopeManager, DocumentScopeManager>("DocumentScope", "文档区域", typeof(DocumentScope))
				//.Add<IDocumentService, DocumentService>()
				);

			sc.AddManagedScoped<IDocumentService, DocumentService>(IsDataScope: true);

			//sc.GenerateEntityManager("DocumentCategory");
			//sc.GenerateEntityManager("Document");

			//sc.AddAutoEntityType(
			//	(TablePrefix ?? "") + "Doc",
			//	false,
			//	typeof(Document),
			//	typeof(DocumentInternal),
			//	typeof(DocumentEditable),
			//	typeof(Category),
			//	typeof(CategoryInternal)
			//	);


			sc.AddDataModules<
				SF.Common.Documents.DataModels.DataDocumentScope, 
				SF.Common.Documents.DataModels.DataDocument,
				SF.Common.Documents.DataModels.DataDocumentCategory,
				SF.Common.Documents.DataModels.DataDocumentAuthor,
				SF.Common.Documents.DataModels.DataDocumentTag,
				SF.Common.Documents.DataModels.DataDocumentTagReference
				>(TablePrefix ?? "Common");

			//sc.AddAutoEntityTest(NewDocumentManager);
			//sc.AddAutoEntityTest(NewDocumentCategoryManager);
			sc.InitServices("Documents", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<IDocumentManager, DocumentManager>(null)
					.WithMenuItems("内容管理/文档管理")
					.Ensure(sp, parent);

				 await sim.DefaultService<IDocumentCategoryManager, DocumentCategoryManager>(null)
					.WithMenuItems("内容管理/文档管理")
					.Ensure(sp, parent);

				 await sim.DefaultService<IDocumentService, DocumentService>(
					 null
					 )
					 .WithMenuItems("内容管理/文档管理")
					 .Ensure(sp, parent);
				 await sim.DefaultService<IDocumentScopeManager, DocumentScopeManager>(null)
					.WithMenuItems("内容管理/文档管理")
					.Ensure(sp, parent);
				 await sp.Resolve<IDocumentScopeManager>().EnsureEntity(
					 ObjectKey.From("default"),
					 s =>
					 {
						 s.Id = "default";
						 s.Name = "默认文档";
					 });

				 await sp.Resolve<IDocumentScopeManager>().EnsureEntity(
					 ObjectKey.From("sys"),
					 s =>
					 {
						 s.Id = "sys";
						 s.Name = "系统文档";
					 });

				 await sp.Resolve<IDocumentScopeManager>().EnsureEntity(
					  ObjectKey.From("help"),
					  s =>
					  {
						  s.Id = "help";
						  s.Name = "帮助文档";
					  });
			 });
			
			return sc;
		}
	}
}

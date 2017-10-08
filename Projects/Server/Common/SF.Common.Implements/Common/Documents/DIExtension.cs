using SF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
using SF.Core.ServiceManagement;
using SF.Common.Documents.Management;
using SF.Common.Documents;
using SF.Core.ServiceManagement.Management;
using SF.Entities.AutoEntityProvider;
using SF.Entities.Tests;

namespace SF.Core.ServiceManagement
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
					//.Add<IDocumentService, DocumentService>()
				);

			sc.GenerateEntityManager("DocumentCategory");
			sc.GenerateEntityManager("Document");

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
				SF.Common.Documents.DataModels.Document,
				SF.Common.Documents.DataModels.DocumentCategory,
				SF.Common.Documents.DataModels.DocumentAuthor,
				SF.Common.Documents.DataModels.DocumentTag,
				SF.Common.Documents.DataModels.DocumentTagReference
				>(TablePrefix);

			//sc.AddAutoEntityTest(NewDocumentManager);
			sc.AddAutoEntityTest(NewDocumentCategoryManager);
			return sc;
		}

		public static IServiceInstanceInitializer<IDocumentManager> NewDocumentManager(
			this IServiceInstanceManager manager
			)
		{
			return manager.Service<IDocumentManager, DocumentManager>(null);
		}
		public static IServiceInstanceInitializer<IDocumentCategoryManager> NewDocumentCategoryManager(
		   this IServiceInstanceManager manager
		   )
		{
			return manager.Service<IDocumentCategoryManager, DocumentCategoryManager>(null);
		}
		public static IServiceInstanceInitializer NewDocumentService(
			this IServiceInstanceManager manager,
			IServiceInstanceInitializer<IDocumentManager> docManager = null,
			IServiceInstanceInitializer<IDocumentCategoryManager> catManager = null
			)
		{
			if (docManager == null)
				docManager = manager.NewDocumentManager();
			if (catManager == null)
				catManager = manager.NewDocumentCategoryManager();

			var svc = manager.DefaultService<IDocumentService, DocumentService>(
				null,
				docManager,
				catManager
				);
			return svc;
		}
	}
}

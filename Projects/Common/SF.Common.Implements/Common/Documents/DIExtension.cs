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


namespace SF.Core.ServiceManagement
{
	public static class DocumentDIExtension
		
	{
		public static IServiceCollection AddDocumentServices(this IServiceCollection sc,string TablePrefix=null)
		{
			//文章
			sc.AddManagedScoped<IDocumentCategoryManager, DocumentCategoryManager>();
			sc.AddManagedScoped<IDocumentManager, DocumentManager>();
			sc.AddManagedScoped<IDocumentService, DocumentService>();

			sc.AddDataModules<
				SF.Common.Documents.DataModels.Document,
				SF.Common.Documents.DataModels.DocumentCategory,
				SF.Common.Documents.DataModels.DocumentAuthor,
				SF.Common.Documents.DataModels.DocumentTag,
				SF.Common.Documents.DataModels.DocumentTagReference
				>(TablePrefix);

			return sc;
		}
	}
}

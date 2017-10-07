using SF.Auth.Identities;
using SF.Core;
using SF.Core.CallPlans;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Metadata;
using SF.Users.Members.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SF.Common.Documents.Management
{
	public class DocumentManager :
		DocumentManager<DocumentInternal, DocumentEditable>,
		IDocumentManager
	{
		public DocumentManager(IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory) : base(DataSetAutoEntityProviderFactory)
		{
		}
	}
	public class DocumentManager<TDocumentInternal, TDocumentEditable> :
		AutoEntityManager<ObjectKey<long>, TDocumentInternal, TDocumentInternal, TDocumentEditable, DocumentQueryArguments, DataModels.Document>,
		IDocumentManager<TDocumentInternal, TDocumentEditable>
		where TDocumentInternal : DocumentInternal
		where TDocumentEditable : DocumentEditable
	{
		public DocumentManager(IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory) : base(DataSetAutoEntityProviderFactory)
		{
		}
	}

}

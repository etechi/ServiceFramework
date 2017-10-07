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
	public class DocumentCategoryManager :
		DocumentCategoryManager<CategoryInternal>,
		IDocumentCategoryManager
	{
		public DocumentCategoryManager(IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory) : base(DataSetAutoEntityProviderFactory)
		{
		}
	}
	public class DocumentCategoryManager<TCategoryInternal> :
		AutoEntityManager<ObjectKey<long>, TCategoryInternal, TCategoryInternal, TCategoryInternal, DocumentCategoryQueryArgument, DataModels.DocumentCategory>,
		IDocumentCategoryManager<TCategoryInternal>
		where TCategoryInternal : CategoryInternal
	{
		public DocumentCategoryManager(IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory) : base(DataSetAutoEntityProviderFactory)
		{
		}
	}

}

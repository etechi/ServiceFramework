using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndServices
{
	public class ContentQueryArguments
	{
		public string Category { get; set; }

	}
	public interface IContentManager<TContent> :
		IEntityManager<int,TContent>,
		IEntitySource<int,TContent, ContentQueryArguments>,
		IContentLoader
		where TContent:Content
	{
		Task<QueryResult<TContent>> Query(ContentQueryArguments args,Paging paging);
	}

}

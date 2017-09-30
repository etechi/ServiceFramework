using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Management.FrontEndContents
{
	public class ContentQueryArgument : IQueryArgument<Option<long>>
	{
		public Option<long> Id { get; set; }
		public string Category { get; set; }
		public string Name { get; set; }
	}
	public interface IContentManager : IContentManager<Content>
	{ }
	public interface IContentManager<TContent> :
		IEntityManager<ObjectKey<long>, TContent>,
		IEntitySource<ObjectKey<long>, TContent, ContentQueryArgument>,
		IContentLoader
		where TContent:Content
	{
	}

}

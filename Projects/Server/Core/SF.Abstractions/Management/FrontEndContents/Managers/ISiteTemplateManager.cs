using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
namespace SF.Management.FrontEndContents
{
	public class SiteTemplateQueryArgument : IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string Name { get; set; }
	}
	public interface ISiteTemplateManager : ISiteTemplateManager<SiteTemplate>
	{ }
	public interface ISiteTemplateManager<TSiteTemplate> :
		IEntityManager<TSiteTemplate>,
		IEntitySource<TSiteTemplate, SiteTemplateQueryArgument>,
		ISiteConfigLoader
		where TSiteTemplate : SiteTemplate
	{
	}
}

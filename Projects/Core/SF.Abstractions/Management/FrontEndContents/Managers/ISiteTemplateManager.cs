using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
namespace SF.Management.FrontEndContents
{
	public interface ISiteTemplateManager : ISiteTemplateManager<SiteTemplate>
	{ }
	public interface ISiteTemplateManager<TSiteTemplate> :
		IEntityManager<long, TSiteTemplate>,
		IEntitySource<long,TSiteTemplate>,
		ISiteConfigLoader
		where TSiteTemplate : SiteTemplate
	{
	}
}

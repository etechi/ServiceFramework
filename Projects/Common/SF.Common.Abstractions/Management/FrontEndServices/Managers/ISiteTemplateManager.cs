using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

namespace ServiceProtocol.Biz.UIManager
{
	public interface ISiteTemplateManager<TSiteTemplate> :
		IServiceObjectManager<int, TSiteTemplate>,
		ISiteConfigLoader
		where TSiteTemplate : SiteTemplate
	{
		Task<TSiteTemplate[]> List();
	}
}

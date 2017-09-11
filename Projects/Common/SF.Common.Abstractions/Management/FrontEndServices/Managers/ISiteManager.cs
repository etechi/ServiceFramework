using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

namespace ServiceProtocol.Biz.UIManager
{
	public interface ISiteManager<TSite>:
		IServiceObjectManager<string,TSite>,
		ISiteResolver
		where TSite:Site
	{
		Task<TSite[]> List();
	}
}

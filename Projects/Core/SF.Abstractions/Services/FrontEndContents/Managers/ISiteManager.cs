using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
namespace SF.Services.FrontEndContents
{
	public interface ISiteManager : 
		ISiteManager<Site>
	{ }
	public interface ISiteManager<TSite>:
		IEntityManager<string,TSite>,
		IEntitySource<string,TSite>,
		ISiteResolver
		where TSite:Site
	{
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
namespace SF.Management.FrontEndContents
{
	public interface ISiteManager : 
		ISiteManager<Site>
	{ }
	public interface ISiteManager<TSite>:
		IEntityManager<ObjectKey<string>, TSite>,
		IEntitySource<ObjectKey<string>, TSite>,
		ISiteResolver
		where TSite:Site
	{
	}
}

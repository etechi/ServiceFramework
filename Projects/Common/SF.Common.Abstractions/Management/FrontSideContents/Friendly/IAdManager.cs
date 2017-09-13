using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Management.FrontEndContents.Friendly
{
	public interface IAdManager
	{
		[Comment(Name = "PC广告位")]
		Task<AdArea[]> List();
		Task<AdArea> Load(long Id);
		Task Save(AdArea value);
	}
	[EntityManager]
	public interface IPCAdManager : IAdManager
	{ }
	[EntityManager]
	public interface IMobileAdManager : IAdManager
	{ }



}

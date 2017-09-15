using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Management.FrontEndContents.Friendly
{
	[Comment("PC广告管理")]
	public interface IPCAdManager : IItemGroupListManager<ImageItem>
	{ }
	[Comment("移动端广告管理")]
	public interface IMobileAdManager : IItemGroupListManager<ImageItem>
	{ }



}

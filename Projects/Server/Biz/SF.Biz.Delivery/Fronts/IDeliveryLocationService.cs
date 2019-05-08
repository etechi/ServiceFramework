using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    /// <summary>
    /// 地区
    /// </summary>
    [EntityObject]
    public class Location
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 全称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 父区域
        /// </summary>
		public long? ParentId { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
		public string ZipCode { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
		public int Level { get; set; }
    }
    /// <summary>
    /// 地区服务
    /// </summary>
    [NetworkService]
    [AnonymousAllowed]
    public interface IDeliveryLocationService
	{
		Task<Location> Get(long Id);
		Task<Location[]> List(long? ParentId);
	}
}

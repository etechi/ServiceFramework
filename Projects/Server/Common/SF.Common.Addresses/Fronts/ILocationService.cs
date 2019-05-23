using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.NetworkService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Common.Addresses
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
        public int Id { get; set; }

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
		public int? ParentId { get; set; }

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
    public interface ILocationService
	{
		Task<Location> Get(int Id);
		Task<Location[]> List(int? ParentId);
	}

    public static class LocationServiceExtensions
    {
        public static async Task<Location[]> GetPath(this ILocationService Service, int Id)
        {
            var l = await Service.Get(Id);
            var list = new List<Location>() { l };
            while(l.ParentId.HasValue)
            {
                var p = await Service.Get(l.ParentId.Value);
                list.Add(p);
                l = p;
            }
            list.Reverse();
            return list.ToArray();
        }
    }
}

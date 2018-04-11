using SF.Sys;
using SF.Sys.Caching;
using SF.Sys.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Utils.ShortLinks
{
	[Table("ShortLink")]
	public class DataShortLink : DataObjectEntityBase<string>
	{
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[MaxLength(100)]
		public override string Id { get; set; }

		/// <summary>
		/// 目标
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		/// Post数据
		/// </summary>
		public string PostData { get; set; }

	
		/// <summary>
		/// 过期时间
		/// </summary>
		public DateTime ExpireTime { get; set; }

	}
}

using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SF.Services.Settings
{
	[Comment("设置服务")]
	public interface ISettingService<T>
	{
		T Value { get; }
	}
}

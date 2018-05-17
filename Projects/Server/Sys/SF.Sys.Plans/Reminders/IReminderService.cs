#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.NetworkService;

namespace SF.Sys.Reminders
{
	public interface IRemindContext
	{
		long? ServiceScopeId { get; }
		string BizType { get;  }
		string BizIdentType { get; }
		long BizIdent { get; }
		object Argument { get; }
		string Message { get; set; }
		string RemindableName { get; }
		string Data { get; set; }
		DateTime? NextRemindTime { get; set; }
		
		DateTime Time { get;  }
	}


	public interface IRemindable
	{
		Task Remind(IRemindContext Context);
	}
	public interface IRemindableDefination
	{
		string Name { get; }

		int RetryMaxCount { get; }
		int RetryDelayStart { get; }
		int RetryDelayStep { get; }
		int RetryDelayMax { get; }

		IRemindable CreateRemindable(IServiceProvider ServiceProvider, long? ScopeId);
	}

	public class RemindSetupArgument
	{
		public string BizType { get; set; }
		public string BizIdentType { get; set; }
		public long BizIdent { get; set; }
		public string RemindableName { get; set; }
		public string Name { get; set; }
		public long? UserId { get; set; }
		public string RemindData { get; set; }
		public DateTime RemindTime { get; set; }
	}

	/// <summary>
	/// 提醒服务
	/// </summary>
	public interface IRemindService
	{
		Task<long> Setup(RemindSetupArgument Argument);

		Task<bool> Remove(long Id);
		Task<bool> Remove(string BizType,string BizIdentType, long BizIdent);
		Task Remind(long Id, object Argument);
		Task Remind(string BizType, string BizIdentType, long BizIdent, object Argument);
		Task<T> Sync<T>(string BizType, string BizIdentType, long BizIdent, Func<Task<T>> Callback);
	}
}

﻿#region Apache License Version 2.0
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
using SF.Sys.Entities;
namespace SF.Sys.Reminders
{ 
	//public class RemindActionInfo
	//{
	//	public string Name { get; set; }
	//	public string Action { get; set; }
	//	public string Description { get; set; }
	//}
	//public interface IRemindAction
	//{
	//	DateTime Time { get; }
	//	string BizIdent { get; }
	//	Task<RemindActionInfo> GetInfo();
	//	Task Execute(long RecordId,bool RetryMode);
	//}
	//public class DelegateRemindAction : IRemindAction
	//{
	//	public DateTime Time { get; set; }
	//	public string BizIdent { get; set; }
	//	public Func<Task<RemindActionInfo>> InfoGetter { get; set; }
	//	public Func<long, bool, Task> Executor { get; set; }
	//	public Task<RemindActionInfo> GetInfo() => InfoGetter();
	//	public Task Execute(long RecordId, bool RetryMode) => Executor(RecordId, RetryMode);
	//}

	
}

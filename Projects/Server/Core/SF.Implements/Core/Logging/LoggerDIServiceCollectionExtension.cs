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

using System.Linq;
using SF.Metadata;
using System;
using System.Reflection;
using SF.Core.Serialization;
using System.Collections.Generic;
using SF.Metadata.Models;
using System.ComponentModel.DataAnnotations;
using SF.Core.Logging;

namespace SF.Core.ServiceManagement
{
	public static class LoggerDIServiceCollectionService
	{
		public static IServiceCollection AddLogService(this IServiceCollection sc,ILogService LogService=null)
		{
			sc.AddSingleton<ILogService>(LogService??new LogService());
			sc.Add(new ServiceDescriptor(typeof(ILogger<>), typeof(Logger<>), ServiceImplementLifetime.Scoped));
			return sc;
		}
	}

}

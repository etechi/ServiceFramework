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

using Microsoft.Extensions.Logging;
using SF.Sys.Comments;
using System;

namespace SF.Sys.Logging.Providors.NLog
{
	partial class NLogProvider : ILoggerProvider
	{
		public bool Scoped => false;
		public NLogProvider()
		{

			var path = System.IO.Path.Combine(
				System.AppDomain.CurrentDomain.BaseDirectory,
				"nlog.config"
				);
			global::NLog.LogManager.LogFactory.LoadConfiguration(path);
		}
		

		public IProviderLogger CreateLogger(string categoryName)
		{
			var logger=global::NLog.LogManager.GetLogger(categoryName);
			
			return new Logger(this,logger);
		}
		
		public void Dispose()
		{
		}
	}
}

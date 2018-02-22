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

using SF.Sys.Comments;
using SF.Sys.Entities;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Sys.Services.Management.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Sys.BackEndConsole
{
	public static class BackEndConsoleBuilderExtensions
	{
		public static IServiceCollection AddConsoleBuilder(
			this IServiceCollection sc,
			string Name,
			Func<IServiceProvider, IBackEndConsoleBuildContext, Task> Builder
			)
		{
			sc.AddInitializer("service", "添加管理后台构建器:" + Name, (isp) =>
			{
				var coll = isp.Resolve<IBackEndConsoleBuilderCollection>();
				coll.AddBuilder(Builder);
			});
			return sc;
		}
		

		public static IServiceInstanceInitializer<T> WithConsolePages<T>(
			this IServiceInstanceInitializer<T> sii,
			string MenuPath,
			string ConsoleIdent=null
			)
		{
			ConsoleIdent = ConsoleIdent ?? "default";
			sii.AddAction((sp, sid) =>
			{
				var dmc = sp.Resolve<IBackEndConsoleBuilderCollection>();
				dmc.AddBuilder((isp, ctx) =>
					ctx.AddEntityManager(MenuPath,sid)
				);
				return Task.CompletedTask;
			});
			return sii;
		}
	}
}
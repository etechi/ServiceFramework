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

using SF.Core.Hosting;
using System.Linq.TypeExpressions;

namespace SF.Core.ServiceManagement
{
	public static class HostingDIServiceCollectionService
	{
		public static IServiceCollection AddConsoleDefaultFilePathStructure(this IServiceCollection sc)
		{
			sc.AddSingleton<IDefaultFilePathStructure, ConsoleDefaultFilePathStructure>();
			return sc;
		}
		public static IServiceCollection AddTestFilePathStructure(
			this IServiceCollection sc,
			string BinPath,
			string RootPath
			)
		{
			sc.AddSingleton<IDefaultFilePathStructure>(new TestFilePathStructure
			{
				BinaryPath = BinPath,
				RootPath = RootPath
			});
			return sc;
		}
		public static IServiceCollection AddFilePathResolver(this IServiceCollection sc)
		{
			sc.AddManagedScoped<IFilePathResolver, FilePathResolver>();
			sc.InitDefaultService<IFilePathResolver, FilePathResolver>(
				"初始化文件路径解析服务",
				new
				{
					Setting = new FilePathDefination()
				}
				);
			return sc;
		}
		public static IServiceCollection AddDynamicTypeBuilder(this IServiceCollection sc)
		{
			sc.AddSingleton<IDynamicTypeBuilder, DynamicTypeBuilder>();
			return sc;
		}
	}

}

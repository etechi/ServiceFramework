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

using SF.Sys.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SF.Sys.Linq;
using SF.Sys.Reflection;

namespace SF.Sys.Services
{
	public class ServiceSetupService : IServiceSetupService
	{
		IFilePathResolver PathResolver { get; }
		public ServiceSetupService(IFilePathResolver PathResolver)
		{
			this.PathResolver = PathResolver;
		}
		public Task<T> LoadSetupSetting<T>(T setting, T defaultSetting) where T:new()
		{
			var type = typeof(T);
			var path = PathResolver.Resolve($"config://setup/{type.FullName}.json");
			if (System.IO.File.Exists(path))
				setting = Json.Parse<T>(System.IO.File.ReadAllText(path));
			else
				setting = Poco.Clone(setting);

			if(!defaultSetting.IsDefault())
				foreach(var p in type.AllPublicInstanceProperties())
				{
					if (p.PropertyType.IsDefaultInstance(p.GetValue(setting)))
						p.SetValue(setting, p.GetValue(defaultSetting));
				}

			return Task.FromResult(setting);
		}
	}
}

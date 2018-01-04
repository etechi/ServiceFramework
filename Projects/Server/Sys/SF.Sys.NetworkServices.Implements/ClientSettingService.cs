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


using SF.Sys.Settings;
using SF.Sys.TimeServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Sys.NetworkService
{
	public class ClientSettingService : IClientSettingService
	{
		//System.Collections.Concurrent.ConcurrentDictionary<string, ClientSetting> Cache { get; }
		//	= new System.Collections.Concurrent.ConcurrentDictionary<string, ClientSetting>();
		HttpSetting HttpSetting { get; }
		SystemSetting SystemSetting { get; }
		IEnumerable<IClientSettingProvider> ClientSettingProviders { get; }
		ITimeService TimeService { get; }
		public ClientSettingService(
			ISettingService<HttpSetting> HttpSetting,
			ISettingService<SystemSetting> SystemSetting,
			IEnumerable<IClientSettingProvider> ClientSettingProviders,
			ITimeService TimeService
			)
		{
			this.TimeService = TimeService;
			this.HttpSetting = HttpSetting.Value;
			this.SystemSetting = SystemSetting.Value;
			this.ClientSettingProviders = ClientSettingProviders;
		}
		public async Task<ClientSetting> GetSettings(string ClientId)
		{
			var options = new Dictionary<string, object>();
			foreach (var p in ClientSettingProviders)
				options[p.Name] = await p.GetOption(ClientId);
			var setting = new ClientSetting
			{
				Time= TimeService.Now,
				ApiBase = HttpSetting.GetApiUrlBase(),
				MainDomain = HttpSetting.Domain,
				SystemName = SystemSetting.SystemName,
				Version = SystemSetting.Version,
				ImageBase = HttpSetting.GetImageUrlBase(),
				ResBase = HttpSetting.GetResUrlBase(),
				HttpsMode = HttpSetting.HttpsMode,
				Options = options
			};
			return setting;
		}
	}
}

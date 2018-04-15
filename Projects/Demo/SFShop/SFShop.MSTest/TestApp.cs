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
using SF.Sys.NetworkService;
using System;
using SF.Sys.Services;
using SF.Sys.Data;

using System.Text;
using SF.Sys.Auth;

namespace SFShop.UT
{
	public static class TestAppBuilder
	{
		public static IAppInstanceBuilder Instance { get; } =
			AppBuilder.Init(EnvironmentType.Development).
				With(sc =>
				{
					var rootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\SFShop.Site\\");
					var binPath = System.IO.Path.Combine(rootPath, "bin\\Debug\\netcoreapp2.0\\");
					sc.AddTestFilePathStructure(
						binPath,
						rootPath
						);
					sc.AddAppSettingDefaultDataSourceConfig();
					sc.AddSingleton(new Moq.Mock<IInvokeContext>().Object);
					sc.AddSingleton(new Moq.Mock<IAccessTokenGenerator>().Object);
					sc.AddSingleton(new Moq.Mock<IAccessTokenValidator>().Object);
					sc.AddSingleton(new Moq.Mock<IUploadedFileCollection>().Object);
					sc.AddNetworkService();
					sc.AddLocalClientService();
					//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456"));
					//sc.AddSingleton<ISigningCredentialStore>(
					//	new DefaultSigningCredentialsStore(
					//		new Microsoft.IdentityModel.Tokens.SigningCredentials(
					//			key,
					//			SecurityAlgorithms.HmacSha256
					//			)
					//	));
				});
			
	}

}

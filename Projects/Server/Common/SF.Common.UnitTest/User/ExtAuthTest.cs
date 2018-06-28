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

using System.Threading.Tasks;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.UnitTest;
using SF.Sys.Hosting;
using SF.Auth.IdentityServices;
using SF.Sys.Services;
using SF.Auth.IdentityServices.Externals;
using SF.Sys.Data;

namespace SF.IdentityService.UnitTest
{
	
	public abstract class ExtAuthTest : TestBase
	{
		public ExtAuthTest(IAppInstanceBuilder builder) : base(builder) { }
	
		[TestMethod]
		public async Task 外部认证_注册()
		{
			await NewServiceScope()
				.Use(async (sp,ct) =>
			{
				var cei = sp.Resolve<IClientExtAuthService>();
				var re=await cei.GetAuthArgument("test", "app.ios");
				var ig = sp.Resolve<IIdentGenerator>();
				var code = "C" + await ig.GenerateAsync("外部测试用户ID");
				var are=await cei.AuthCallback(new AuthCallbackArgument
				{
					Arguments = new System.Collections.Generic.Dictionary<string, string>
					{
						{"code",code }
					},
					State = re.State
				});
				Assert.IsNotNull(are);
				Assert.IsNotNull(are.AccessToken);
				Assert.AreEqual("用户"+code,are.User.Name);

				var re1 = await cei.GetAuthArgument("test", "app.ios");
				var are1 = await cei.AuthCallback(new AuthCallbackArgument
				{
					Arguments = new System.Collections.Generic.Dictionary<string, string>
					{
						{"code",code }
					},
					State = re.State
				});
				Assert.IsNotNull(are1);
				Assert.IsNotNull(are1.AccessToken);
				Assert.AreNotEqual(are.AccessToken, are1.AccessToken);
				Assert.AreEqual(are.User.Id, are1.User.Id);
				Assert.AreEqual(are.User.Name, are1.User.Name);
			}
			);
		}
	
	
	}


}

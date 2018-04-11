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

using SF.Sys.Entities.AutoEntityProvider;
using SF.Utils.ShortLinks;

namespace SF.Sys.Services
{
	public static class ShortLinkDIExtension
		
	{
		public static IServiceCollection AddShortLinkService(this IServiceCollection sc,string TablePrefix=null)
		{
			//短链接
			sc.EntityServices(
				"ShortLink",
				"短链接管理",
				d => d.Add<IShortLinkManager, ShortLinkManager>("ShortLink", "短链接", typeof(ShortLink))
					//.Add<ICommentService, CommentService>()
				);

			sc.AddDataModules<
				DataShortLink
				>(TablePrefix ?? "Util");

			sc.AddTransient(sp => (IShortLinkCreateService)sp.Resolve<IShortLinkManager>());
			sc.AddTransient(sp => (IShortLinkService)sp.Resolve<IShortLinkManager>());

			//sc.AddAutoEntityTest(NewCommentManager);
			//sc.AddAutoEntityTest(NewCommentCategoryManager);
			sc.InitServices("ShortLinks", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<IShortLinkManager, ShortLinkManager>(null).Ensure(sp, parent);

			 });
			return sc;
		}

		
	}
}

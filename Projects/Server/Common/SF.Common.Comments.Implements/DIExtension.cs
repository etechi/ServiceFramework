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

using SF.Common.Comments.Management;
using SF.Common.Comments;
using SF.Sys.Services.Management;
using SF.Sys.Entities.AutoTest;
using SF.Sys.Entities.AutoEntityProvider;

namespace SF.Sys.Services
{
	public static class CommentDIExtension
		
	{
		public static IServiceCollection AddCommentServices(this IServiceCollection sc,string TablePrefix=null)
		{
			//文章
			sc.EntityServices(
				"Comment",
				"文档管理",
				d => d.Add<ICommentManager, CommentManager>("Comment", "文档", typeof(Comment))
					//.Add<ICommentService, CommentService>()
				);

			sc.AddManagedScoped<ICommentService, CommentService>(IsDataScope: true);
			sc.GenerateEntityManager("Comment");

			//sc.AddAutoEntityType(
			//	(TablePrefix ?? "") + "Doc",
			//	false,
			//	typeof(Comment),
			//	typeof(CommentInternal),
			//	typeof(CommentEditable),
			//	typeof(Category),
			//	typeof(CategoryInternal)
			//	);


			sc.AddDataModules<
				SF.Common.Comments.DataModels.Comment
				>(TablePrefix ?? "Common");

			//sc.AddAutoEntityTest(NewCommentManager);
			//sc.AddAutoEntityTest(NewCommentCategoryManager);
			sc.InitServices("Comment", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<ICommentManager, CommentManager>(null).Ensure(sp, parent);
				 await sim.DefaultService<ICommentService, CommentService>(null).Ensure(sp, parent);

			 });
			return sc;
		}

		
	}
}

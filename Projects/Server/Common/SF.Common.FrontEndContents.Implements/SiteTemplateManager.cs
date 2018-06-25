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

using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.FrontEndContents
{
	public class SiteTemplateManager : 
		SiteTemplateManager<SiteTemplate, DataModels.Site, DataModels.SiteTemplate>,
		ISiteTemplateManager
	{
		public SiteTemplateManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
	}
	public class SiteTemplateManager<TSiteTemplatePublic,TSite,TSiteTemplate> :
		ModidifiableEntityManager<ObjectKey<long>, TSiteTemplatePublic, SiteTemplateQueryArgument, TSiteTemplatePublic, TSiteTemplate>,
		ISiteTemplateManager<TSiteTemplatePublic>
		where TSiteTemplatePublic : SiteTemplate, new()
		where TSite : DataModels.Site<TSite,TSiteTemplate>
		where TSiteTemplate : DataModels.SiteTemplate<TSite,TSiteTemplate>, new()
	{

        protected override async Task<TSiteTemplatePublic> OnMapModelToEditable(IDataContext DataContext, IQueryable<TSiteTemplate> Query)
		{
			var re=await Query.Select(s =>
				new
				{
					tmpl = new TSiteTemplatePublic
					{
						Id = s.Id,
						Name = s.Name,
					},
					data=s.Data
				}).SingleOrDefaultAsync();
			if (re == null)
				return null;
			re.tmpl.Model = Json.Parse<SiteConfigModels.SiteModel>(re.data);
			return re.tmpl;
		}

        protected override IQueryable<TSiteTemplatePublic> OnMapModelToDetail(IQueryable<TSiteTemplate> Query)
		{
			return Query.Select(s => new TSiteTemplatePublic
			{
				Id = s.Id,
				Name = s.Name,
				
			});
		}

		protected override IQueryable<TSiteTemplate> OnBuildQuery(IQueryable<TSiteTemplate> Query, SiteTemplateQueryArgument Arg)
		{
			return Query.Filter(Arg.Name, c => c.Name);
			
		}
		protected override PagingQueryBuilder<TSiteTemplate> PagingQueryBuilder => new PagingQueryBuilder<TSiteTemplate>(
			"name",
			b => b.Add("name", c => c.Name, true)
			);
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Name = obj.Name;
			Model.Data = Json.Stringify(obj.Model);
			return Task.CompletedTask;
		}

        public SiteTemplateManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
        }

		
		public Task<string> LoadConfig(long templateId)
		{
			return DataScope.Use(
				"查找站点配置",
				ctx => ctx.Queryable<DataModels.SiteTemplate>()
				.Where(t => t.Id == templateId)
				.Select(t => t.Data)
				.SingleOrDefaultAsync()
				);
		}

    }
}

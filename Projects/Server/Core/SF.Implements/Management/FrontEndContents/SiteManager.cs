﻿#region Apache License Version 2.0
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
using SF.Data;
using SF.Management.FrontEndContents.DataModels;

namespace SF.Management.FrontEndContents
{
	public class SiteManager : 
		SiteManager<Site, DataModels.Site, DataModels.SiteTemplate>,
		ISiteManager
	{
		public SiteManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
	}

	public class SiteManager<TSitePublic,TSite,TSiteTemplate> :
		ModidifiableEntityManager<string, TSitePublic, TSitePublic,TSite>,
		ISiteManager<TSitePublic>
		where TSitePublic:Site,new()
		where TSite : DataModels.Site<TSite,TSiteTemplate>, new() 
		where TSiteTemplate : DataModels.SiteTemplate<TSite,TSiteTemplate>
	{

        protected override async Task<TSitePublic> OnMapModelToEditable(IContextQueryable<TSite> Query)
		{
			return await Query.Select(s => new TSitePublic
				{
					Id = s.Id,
					Name = s.Name,
					TemplateId = s.TemplateId
				}).SingleOrDefaultAsync();
		}

        protected override IContextQueryable<TSitePublic> OnMapModelToDetail(IContextQueryable<TSite> Query)
		{
			return Query.Select(s => new TSitePublic
			{
				Id = s.Id,
				Name = s.Name,
				TemplateId = s.TemplateId,
				TemplateName = s.Template.Name
			});
		}

		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Id = obj.Id;
			Model.Name = obj.Name;
			Model.TemplateId = obj.TemplateId;
			return Task.CompletedTask;
        }
        protected override Task OnRemoveModel(IModifyContext ctx)
        {
            var mid = ctx.Model.Id;
            return base.OnRemoveModel(ctx);
        }

        public SiteManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		public async Task<long> FindTemplateId(string site)
		{
			return await DataSet.AsQueryable()
				.Where(s => s.Id == site)
				.Select(s => s.TemplateId)
				.SingleOrDefaultAsync();
		}

		public async Task<TSitePublic[]> List()
		{
			return await OnMapModelToDetail(DataSet.AsQueryable()).ToArrayAsync();
		}

    }
}

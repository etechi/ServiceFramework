using SF.Core;
using SF.Data;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents
{
	public class SiteTemplateManager : 
		SiteTemplateManager<SiteTemplate, DataModels.Site, DataModels.SiteTemplate>,
		ISiteTemplateManager
	{
		public SiteTemplateManager(IDataSetEntityManager<DataModels.SiteTemplate> EntityManager) : base(EntityManager)
		{
		}
	}
	public class SiteTemplateManager<TSiteTemplatePublic,TSite,TSiteTemplate> :
		EntityManager<long, TSiteTemplatePublic, SiteTemplateQueryArgument, TSiteTemplatePublic, TSiteTemplate>,
		ISiteTemplateManager<TSiteTemplatePublic>
		where TSiteTemplatePublic : SiteTemplate, new()
		where TSite : DataModels.Site<TSite,TSiteTemplate>
		where TSiteTemplate : DataModels.SiteTemplate<TSite,TSiteTemplate>, new()
	{

        protected override async Task<TSiteTemplatePublic> OnMapModelToEditable(IContextQueryable<TSiteTemplate> Query)
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

        protected override IContextQueryable<TSiteTemplatePublic> OnMapModelToDetail(IContextQueryable<TSiteTemplate> Query)
		{
			return Query.Select(s => new TSiteTemplatePublic
			{
				Id = s.Id,
				Name = s.Name,
				
			});
		}

		protected override IContextQueryable<TSiteTemplate> OnBuildQuery(IContextQueryable<TSiteTemplate> Query, SiteTemplateQueryArgument Arg, Paging paging)
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

        public SiteTemplateManager(IDataSetEntityManager<TSiteTemplate> EntityManager) : base(EntityManager)
		{
        }

		
		public async Task<string> LoadConfig(long templateId)
		{
			return await DataSet.AsQueryable()
				.Where(t => t.Id == templateId)
				.Select(t => t.Data)
				.SingleOrDefaultAsync();
		}

    }
}

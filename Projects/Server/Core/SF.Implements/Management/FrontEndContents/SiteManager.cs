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
		public SiteManager(IDataSetEntityManager<Site,DataModels.Site> EntityManager) : base(EntityManager)
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

        public SiteManager(IDataSetEntityManager<TSitePublic,TSite> EntityManager) : base(EntityManager)
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

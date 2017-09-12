using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
using SF.Data;
namespace SF.Management.FrontEndServices
{
	public class SiteManager<TSitePublic,TSite,TSiteTemplate> :
		EntityManager<string,TSitePublic, TSitePublic,TSite>,
		ServiceProtocol.Biz.UIManager.ISiteManager<TSitePublic>
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

        protected override IContextQueryable<TSitePublic> OnMapModelToPublic(IContextQueryable<TSite> Query)
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

        public SiteManager(IDataSetEntityManager<TSite> EntityManager) : base(EntityManager)
		{
		}

		public async Task<int> FindTemplateId(string site)
		{
			return await DataSet.AsQueryable()
				.Where(s => s.Id == site)
				.Select(s => s.TemplateId)
				.SingleOrDefaultAsync();
		}

		public async Task<TSitePublic[]> List()
		{
			return await OnMapModelToPublic(DataSet.AsQueryable()).ToArrayAsync();
		}

    }
}

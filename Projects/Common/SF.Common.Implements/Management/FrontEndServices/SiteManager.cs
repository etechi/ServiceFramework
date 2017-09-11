using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

using ServiceProtocol.Data.Entity;

namespace ServiceProtocol.Biz.UIManager.Entity
{
    [DataObjectLoader("界面站点")]
	public class SiteManager<TSitePublic,TSite,TSiteTemplate> :
		ObjectManager.EntityServiceObjectManager<string,TSitePublic, TSitePublic,TSite>,
		ServiceProtocol.Biz.UIManager.ISiteManager<TSitePublic>,
        IDataObjectLoader
		where TSitePublic:Site,new()
		where TSite : Models.Site<TSite,TSiteTemplate>, new() 
		where TSiteTemplate : Models.SiteTemplate<TSite,TSiteTemplate>
	{

        protected override async Task<TSitePublic> MapModelToEditable(IContextQueryable<TSite> Query)
		{
			return await Query.Select(s => new TSitePublic
				{
					Id = s.Id,
					Name = s.Name,
					TemplateId = s.TemplateId
				}).SingleOrDefaultAsync();
		}

        protected override IContextQueryable<TSitePublic> MapModelToInternal(IContextQueryable<TSite> Query)
		{
			return Query.Select(s => new TSitePublic
			{
				Id = s.Id,
				Name = s.Name,
				TemplateId = s.TemplateId,
				TemplateName = s.Template.Name
			});
		}

		protected override Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Id = obj.Id;
			Model.Name = obj.Name;
			Model.TemplateId = obj.TemplateId;
            ctx.AddPostAction(() =>
                Engine.NotifySiteBindChanged(obj.Id)
                );
			return Task.CompletedTask;
        }
        protected override Task OnRemoveModel(ModifyContext ctx)
        {
            var mid = ctx.Model.Id;
            ctx.AddPostAction(() =>
                Engine.NotifySiteBindChanged(mid)
                );
            return base.OnRemoveModel(ctx);
        }
        public ISiteRenderEngine Engine { get; }

        public SiteManager(IDataContext context, ISiteRenderEngine Engine,Lazy<IModifyFilter> ModifyFilter) : base(context, ModifyFilter)
		{
            this.Engine = Engine;
		}

		public async Task<int> FindTemplateId(string site)
		{
			return await Context
				.ReadOnly<TSite>()
				.Where(s => s.Id == site)
				.Select(s => s.TemplateId)
				.SingleOrDefaultAsync();
		}

		public async Task<TSitePublic[]> List()
		{
			return await MapModelToInternal(Context.ReadOnly<TSite>()).ToArrayAsync();
		}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

using ServiceProtocol.Data.Entity;

namespace ServiceProtocol.Biz.UIManager.Entity
{
    [DataObjectLoader("界面站点模板")]
	public class SiteTemplateManager<TSiteTemplatePublic,TSite,TSiteTemplate> :
		ObjectManager.EntityServiceObjectManager<int, TSiteTemplatePublic, TSiteTemplatePublic, TSiteTemplate>,
		ServiceProtocol.Biz.UIManager.ISiteTemplateManager<TSiteTemplatePublic>,
        IDataObjectLoader
		where TSiteTemplatePublic :SiteTemplate, new()
		where TSite : Models.Site<TSite,TSiteTemplate>
		where TSiteTemplate : Models.SiteTemplate<TSite,TSiteTemplate>, new()
	{

        protected override async Task<TSiteTemplatePublic> MapModelToEditable(IContextQueryable<TSiteTemplate> Query)
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
			re.tmpl.Model = Json.Decode<SiteConfigModels.SiteModel>(re.data);
			return re.tmpl;
		}

        protected override IContextQueryable<TSiteTemplatePublic> MapModelToInternal(IContextQueryable<TSiteTemplate> Query)
		{
			return Query.Select(s => new TSiteTemplatePublic
			{
				Id = s.Id,
				Name = s.Name,
				
			});
		}
        protected override Task OnRemoveModel(ModifyContext ctx)
        {
            var mid = ctx.Model.Id;
            ctx.AddPostAction(() =>
                Engine.NotifySiteTemplateChanged(mid)
                );
            return base.OnRemoveModel(ctx);
        }

		protected override Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Name = obj.Name;
			Model.Data = Json.Encode(obj.Model);
            ctx.AddPostAction(() =>
                Engine.NotifySiteTemplateChanged(obj.Id)
                );
			return Task.CompletedTask;
		}
        public ISiteRenderEngine Engine { get; }

        public SiteTemplateManager(IDataContext context, ISiteRenderEngine Engine,Lazy<IModifyFilter> ModifyFilter) : base(context, ModifyFilter)
		{
            this.Engine = Engine;

        }
		public async Task<TSiteTemplatePublic[]> List()
		{
			return await MapModelToInternal(Context.ReadOnly<TSiteTemplate>()).ToArrayAsync();
		}

		public async Task<string> LoadConfig(int templateId)
		{
			return await Context.ReadOnly<TSiteTemplate>()
				.Where(t => t.Id == templateId)
				.Select(t => t.Data)
				.SingleOrDefaultAsync();
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            var re = await DataObjectLoader.Load(
                Keys,
                id => int.Parse(id[0]),
                id => FindByIdAsync(id),
                async (ids) => {
                    var tmps = await MapModelToInternal(Context.ReadOnly<TSiteTemplate>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
                    return tmps;
                });
            return re;
        }

    }
}

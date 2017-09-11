using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndServices
{
    public class ContentManager<TContentPublic,TContent> :
		EntityManger<int,TContentPublic, TContentPublic,TContent>,
		ServiceProtocol.Biz.UIManager.IContentManager<TContentPublic>,
        IDataObjectLoader
		where TContentPublic : Content,new()
		where TContent: Models.Content, new() 
	{

        protected override async Task<TContentPublic> MapModelToEditable(IContextQueryable<TContent> Query)
		{
			var re = await Query.Select(m => new
			{
				content = new TContentPublic
				{
					Image = m.Image,
					Icon = m.Icon,
					FontIcon = m.FontIcon,
					Uri = m.Uri,
					UriTarget = m.UriTarget,
					Title1 = m.Title1,
					Title2 = m.Title2,
					Title3 = m.Title3,
					Summary = m.Summary,
					Id = m.Id,
					Name = m.Name,
					Category = m.Category,
					ProviderType = m.ProviderType,
					ProviderConfig = m.ProviderConfig,
					Disabled = m.Disabled
				},
				items = m.ItemsData
			}).SingleOrDefaultAsync();
			if (re == null)
				return null;
			if (!string.IsNullOrEmpty(re.items))
				re.content.Items = Json.Decode<ContentItem[]>(re.items);
			return re.content;
		}

        protected override IContextQueryable<TContentPublic> MapModelToInternal(IContextQueryable<TContent> Query)
		{
			return Query.Select(m => new TContentPublic
			{
				Image = m.Image,
				Icon = m.Icon,
				FontIcon = m.FontIcon,
				Uri = m.Uri,
				UriTarget = m.UriTarget,
				Title1 = m.Title1,
				Title2 = m.Title2,
				Title3 = m.Title3,
				Summary = m.Summary,
				Id = m.Id,
				Name = m.Name,
				Category = m.Category,
				ProviderType = m.ProviderType,
				ProviderConfig = m.ProviderConfig,
				Disabled = m.Disabled
			});
		}

		protected override Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Image = obj.Image;
			Model.Icon = obj.Icon;
			Model.FontIcon = obj.FontIcon;
			Model.Uri = obj.Uri;
			Model.UriTarget = obj.UriTarget;
			Model.Title1 = obj.Title1;
			Model.Title2 = obj.Title2;
			Model.Title3 = obj.Title3;
			Model.Summary = obj.Summary;
			Model.Id = obj.Id;
			Model.Name = obj.Name;
			Model.Category = obj.Category;
			Model.ProviderType = obj.ProviderType;
			Model.ProviderConfig = obj.ProviderConfig;
			Model.Disabled = obj.Disabled;
			Model.ItemsData = obj.Items == null ? null : Json.Encode(obj.Items);
            var mid = Model.Id;
            ctx.AddPostAction(() => 
                Engine.NotifyContentChanged(mid)
                );
			return Task.CompletedTask;
		}
        protected override Task OnRemoveModel(ModifyContext ctx)
        {
            var id = ctx.Model.Id;
            ctx.AddPostAction(() =>
                Engine.NotifyContentChanged(id)
                );
            return base.OnRemoveModel(ctx);
        }
        static PagingQueryBuilder<TContent> pagingBuilder = new PagingQueryBuilder<TContent>(
			"name",
			i => i.Add("name", m => m.Name));
		public async Task<QueryResult<TContentPublic>> Query(ContentQueryArguments args,Paging paging)
		{
			var q = (IContextQueryable<TContent>)Context.ReadOnly<TContent>();
			if (args.Category != null)
				q = q.Where(c => c.Category == args.Category);
			return await q.ToQueryResultAsync(
				MapModelToInternal,
				r => r,
				pagingBuilder,
				paging
				);
		}

		public async Task<IContent> LoadContent(int contentId)
		{
			return await LoadForEditAsync(contentId);
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            var re = await DataObjectLoader.Load(
                Keys,
                id => int.Parse(id[0]),
                id => FindByIdAsync(id),
                async (ids) => {
                    var tmps = await MapModelToInternal(Context.ReadOnly<TContent>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
                    return tmps;
                });
            return re;
        }

        public ISiteRenderEngine Engine { get; }
		public ContentManager(IDataContext context, ISiteRenderEngine Engine,Lazy<IModifyFilter> ModifyFilter) : base(context, ModifyFilter)
		{
            this.Engine = Engine;

        }

		
	}
}

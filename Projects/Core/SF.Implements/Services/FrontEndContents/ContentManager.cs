using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;
using SF.Core;
using SF.Data;
using SF.Services.FrontEndContents.DataModels;

namespace SF.Services.FrontEndContents
{
	public class ContentManager :
		ContentManager<Content, DataModels.Content>,
		IContentManager
	{
		public ContentManager(IDataSetEntityManager<DataModels.Content> EntityManager) : base(EntityManager)
		{
		}
	}




	public class ContentManager<TContentPublic,TContent> :
		EntityManager<long,TContentPublic, ContentQueryArgument, TContentPublic,TContent>,
		IContentManager<TContentPublic>
		where TContentPublic : Content,new()
		where TContent: DataModels.Content, new() 
	{

        protected override async Task<TContentPublic> OnMapModelToEditable(IContextQueryable<TContent> Query)
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
				re.content.Items = Json.Parse<ContentItem[]>(re.items);
			return re.content;
		}

        protected override IContextQueryable<TContentPublic> OnMapModelToPublic(IContextQueryable<TContent> Query)
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

		protected override Task OnUpdateModel(IModifyContext ctx)
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
			Model.ItemsData = obj.Items == null ? null : Json.Stringify(obj.Items);
            var mid = Model.Id;
			return Task.CompletedTask;
		}
		protected override PagingQueryBuilder<TContent> PagingQueryBuilder { get; }=
			new PagingQueryBuilder<TContent>(
				"name",
				i => i.Add("name", m => m.Name));
		protected override IContextQueryable<TContent> OnBuildQuery(IContextQueryable<TContent> Query, ContentQueryArgument Arg, Paging paging)
		{
			return Query.Filter(Arg.Category,c=>c.Category);
		}

		public async Task<IContent> LoadContent(long contentId)
		{
			return await GetAsync(contentId);
		}

		public ContentManager(IDataSetEntityManager<TContent> EntityManager) : base(EntityManager)
		{

        }

		
	}
}

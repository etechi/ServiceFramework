using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Entities;

namespace SF.Management.FrontEndContents
{
	public static class Ensurer
	{
		public static async Task<Site> SiteEnsure(this ISiteManager SiteManager, string site, string name, long siteTemplateId)
		{
			return await SiteManager.EnsureEntity(
				site,
				() => new Site { Id = site, Name = name, TemplateId = siteTemplateId },
				s =>
				{
					s.Name = name;
					s.TemplateId = siteTemplateId;
				}
				);
		}

		public static async Task<SiteTemplate> SiteTemplateEnsure(
			this ISiteTemplateManager SiteTemplateManager, 
			string name, 
			SiteConfigModels.SiteModel Site
			)
		{
			return await SiteTemplateManager.EnsureEntity(
				   await SiteTemplateManager.QuerySingleEntityIdent(new SiteTemplateQueryArgument { Name = name }),
				   () => new SiteTemplate
				   {
					   Name = name,
					   Model = Site
				   },
				   s =>
				   {
					   s.Model = Site;
				   }
				   );

		}
		public static async Task<TContent> ContentEnsure<TContent>(
			this IContentManager<TContent> ContentManager,
			string category,
			string name,
			ContentItem Data,
			ContentItem[] Items = null,
			string summary = null
			) where TContent:Content,new()
		{
			return await ContentEnsure(ContentManager, category, name, Data, null, null, Items, summary);
		}
		public static async Task<TContent> ContentEnsure<TContent>(
			this IContentManager<TContent> ContentManager,
			string category,
			string name,
			ContentItem Data,
			string ProviderType,
			string ProviderConfig,
			ContentItem[] Items = null,
			string summary = null
			) where TContent : Content,new()
		{
			return await ContentManager.EnsureEntity(
				await ContentManager.QuerySingleEntityIdent(new ContentQueryArgument { Category = category, Name = name }),
				(TContent c) =>
				{
					c.Name = name;
					c.Category = category;
					c.Disabled = false;
					c.ProviderConfig = ProviderConfig;
					c.ProviderType = ProviderType;
					c.Items = Items;

					c.FontIcon = Data?.FontIcon;
					c.Icon = Data?.Icon;
					c.Image = Data?.Image;
					c.Summary = Data?.Summary ?? summary;
					c.Title1 = Data?.Title1;
					c.Title2 = Data?.Title2;
					c.Title3 = Data?.Title3;
					c.Uri = Data?.Uri;
					c.UriTarget = Data?.UriTarget;
				}
				);
		}
		public static async Task<TContent> TitleImageContentEnsure<TContent>(
			this IContentManager<TContent> ContentManager,
			string category,
			string name,
			string image,
			string uri
			) where TContent:Content,new()
		{
			return await ContentManager.ContentEnsure(
				category,
				name,
				new ContentItem
				{
					Title1 = name,
					Image = image,
					Uri = uri
				}
				);
		}
		public static async Task<TContent> ContentEnsure<TContent>(
			this IContentManager<TContent> ContentManager,
			string category,
			string name,
			string loader,
			string config,
			string uri = null
			) where TContent:Content,new()
		{
			return await ContentManager.ContentEnsure(
				category,
				name,
				new ContentItem
				{
					Title1 = name,
					Uri = uri
				},
				loader,
				config
				);
		}
		//public static async Task<Content> CategoryProductContentEnsure(
		//	this IContentManager ContentManager,
		//	string category,
		//	string name,
		//	int cat_id,
		//	string config
		//	)
		//{
		//	return await ContentManager.ContentEnsure(
		//		category,
		//		name,
		//		new ContentItem
		//		{
		//			Title1 = name,
		//			Uri = $"/product/category/{cat_id}"
		//		},
		//		"CategoryProductProvider",
		//		config
		//		);
		//}
	}
}


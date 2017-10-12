using Microsoft.AspNetCore.Mvc;
using SF.Common.Documents;
using SF.Entities;
using SF.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Management.FrontEndContents.Mvc;
using SF.Core.ServiceManagement;

namespace Hygou.Site.Controllers
{
	public class HelpController : BaseController
	{
		public HygouSetting Setting { get; }
		public SF.Common.Documents.IDocumentService DocService { get; }
		public HelpController(ISettingService<HygouSetting> Setting, NamedServiceResolver<SF.Common.Documents.IDocumentService> DocServiceResolver)
		{
			this.Setting = Setting.Value;
			this.DocService = DocServiceResolver("pc-help");
		}
		public ActionResult Index()
		{
			return Redirect("/help/doc/" + Setting.PCHelpCenterDefaultDocId);
		}
        async Task<IEnumerable<Category>> LoadCategories(long? CurrentDocId)
        {
            var cats = (await DocService.ListChildContainersAsync(null, new Paging { Count = 1000 })).Items;
            var docs = (await DocService.ListItemsAsync(null, new Paging { Count = 1000 })).Items;
            var docgrps = docs.Where(d => d.ContainerId != null).GroupBy(d => d.Container.Id).ToDictionary(g => g.Key);
            foreach (var c in cats)
            {
                c.Items = docgrps.Get(c.Id)?
                    .OrderBy(i => i.ItemOrder)
                    .ToArray() ?? 
                    Array.Empty<Document>();
            }
            ViewBag.CurrentDocId = CurrentDocId;
            ViewBag.Categories = cats;
            return cats;
        }
		public async Task<ActionResult> Doc(string id)
		{
			long nid;

			var doc = long.TryParse(id, out nid)?await DocService.GetAsync(ObjectKey.From(nid)):await DocService.GetByKeyAsync(id);
			if (doc == null)
				return NotFound();

			await this.LoadUIPageBlocks("帮助中心");
            var cats = await LoadCategories(doc.Id);

            doc.Container = cats.FirstOrDefault(c => c.Id == doc.ContainerId);
			return View(doc);
		}
        //[Authorize]
        public async Task<ActionResult> Feedback()
        {
            await this.LoadUIPageBlocks("用户反馈");
            await LoadCategories(null);
            return View();
        }
    }
}

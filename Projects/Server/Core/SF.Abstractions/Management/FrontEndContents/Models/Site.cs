using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents
{
	[EntityObject]
    public class Site : IEntityWithId<string>
	{
		[Key]
		[Display(Name = "ID", Prompt = "保存后自动产生")]
		[TableVisible]
		[StringLength(20)]
		public string Id { get; set; }

		[Display(Name = "名称")]
		[TableVisible]
		[StringLength(100)]
		[Required]
		public string Name { get; set; }

		[Display(Name = "站点模板")]
		[EntityIdent(typeof(SiteTemplate), nameof(TemplateName))]
		[Required]
		public long TemplateId { get; set; }

		[Display(Name = "站点模板")]
		[TableVisible]
		[Ignore]
		public string TemplateName { get; set; }

       
    }
}

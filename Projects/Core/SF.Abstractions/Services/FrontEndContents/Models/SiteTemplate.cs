using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.FrontEndContents
{
    [EntityObject("界面站点模板")]
    public class SiteTemplate:IEntityWithId<long>
    {
		[Key]
		[Display(Name = "ID", Prompt = "保存后自动产生")]
		[ReadOnly(true)]
		[TableVisible]
		public long Id { get; set; }

		[TableVisible]
		[Display(Name = "名称")]
		[StringLength(100)]
		[Required]
		public string Name { get; set; }

		//[Display(Name = "模板")]
		[Required]
		public SiteConfigModels.SiteModel Model { get; set; }

    }
}

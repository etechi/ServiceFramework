using ServiceProtocol.Annotations;
using ServiceProtocol.ObjectManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.UIManager.Friendly
{
	public class TextGroupItem<T> :LinkItemBase
        where T:LinkItemBase
	{
        [Display(Name = "分组标题")]
        [Layout(1)]
        [Required]
        public string Text { get; set; }

        [Display(Name ="分组项目")]
        [Layout(2)]
        [TableRows]
        public T[] Items { get; set; }
	}
}

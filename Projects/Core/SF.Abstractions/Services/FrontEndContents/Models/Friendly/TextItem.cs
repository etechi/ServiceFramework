using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.FrontEndContents.Friendly
{
	public class TextItem : LinkItemBase
    {
        [Display(Name = "文字1")]
        [Required]
        [Layout(1)]
        public string Text1 { get; set; }

        [Display(Name = "文字2")]
        [Layout(2)]
        public string Text2 { get; set; }

    }
}

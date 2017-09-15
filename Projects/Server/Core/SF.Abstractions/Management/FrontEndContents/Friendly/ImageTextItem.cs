using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
{
	public class ImageTextItem : LinkItemBase
    {

        [Display(Name = "图片")]
        [Image]
        [Layout(1)]
        public string Image { get; set; }

        [Display(Name = "文字1")]
        [Required]
        [Layout(2)]
        public string Text1 { get; set; }

        [Display(Name = "文字2")]
        [Layout(3)]
        public string Text2 { get; set; }
    }
}

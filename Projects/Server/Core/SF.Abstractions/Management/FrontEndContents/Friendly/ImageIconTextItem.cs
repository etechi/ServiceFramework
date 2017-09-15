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
	public class ImageIconTextItem : LinkItemBase
    {

        [Display(Name = "大图标")]
        [Required]
        [Image]
        [Layout(1)]
        public string Image { get; set; }

        [Display(Name = "小图标")]
        [Required]
        [Image]
        [Layout(1)]
        public string Icon { get; set; }

        [Display(Name = "文字")]
        [Required]
        [Layout(2)]
        public string Text { get; set; }
    }
}

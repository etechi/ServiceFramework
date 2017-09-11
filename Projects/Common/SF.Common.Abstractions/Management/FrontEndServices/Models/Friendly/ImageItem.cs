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
   
	public class ImageItem : LinkItemBase
	{
        [Display(Name = "图片")]
        [Required]
        [Image]
        [Layout(1)]
        public string Image { get; set; }

	}
    
}

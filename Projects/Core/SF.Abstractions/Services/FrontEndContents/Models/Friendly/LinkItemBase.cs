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

	public enum LinkTarget
    {
        [Display(Name = "默认")]
        _default,
        [Display(Name = "新窗口")]
        _blank,
        [Display(Name = "当前页")]
        _self
    }
    public class LinkItemBase
	{
        [Display(Name ="链接")]
        [Layout(100)]
        public string Link { get; set; }

        [Display(Name = "打开位置")]
        [Layout(101)]
        public LinkTarget LinkTarget { get; set; } = LinkTarget._default;
	}
    
}

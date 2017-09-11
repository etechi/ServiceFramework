using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndServices
{
	[Setting(Name ="界面管理器")]
    public class UIManagerSetting
    {
        [EntityIdent("界面内容")]
        [Display(Name="PC头部菜单",GroupName ="PC端")]
        public int PCHeadMenuId { get; set; }

        [EntityIdent("界面内容")]
        [Display(Name = "PC头部产品分类菜单", GroupName = "PC端")]
        public int PCHeadProductCategoryMenuId { get; set; }



        [EntityIdent("界面内容")]
        [Display(Name = "PC首页幻灯片", GroupName = "PC端")]
        public int PCHomePageSliderId { get; set; }

        [EntityIdent("界面内容")]
        [Display(Name = "PC尾部菜单", GroupName = "PC端")]
        public int PCHomeTailMenuId { get; set; }
        [EntityIdent("界面内容")]
        [Display(Name = "PC尾部链接", GroupName = "PC端")]
        public int PCHomeTailLinksId { get; set; }

        [Display(Name = "PC广告位分类", GroupName = "PC端")]
        public string PCAdCategory { get; set; }


        [EntityIdent("界面内容")]
        [Display(Name = "移动端首页幻灯片", GroupName = "移动端")]
        public int MobileHomePageSliderId { get; set; }

        [EntityIdent("界面内容")]
        [Display(Name = "移动端首页链接菜单", GroupName = "移动端")]
        public int MobileHomeIconLinkId { get; set; }

        [EntityIdent("界面内容")]
        [Display(Name = "移动端产品分类菜单", GroupName = "移动端")]
        public int MobileProductCategoryMenuId { get; set; }

        [Display(Name = "移动端广告位分类", GroupName = "移动端")]
        public string MobileAdCategory { get; set; }
    }
}

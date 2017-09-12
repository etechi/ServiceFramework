using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Services.FrontEndContents.Friendly
{
	[EntityObject("界面广告位")]
    public class AdArea
	{
        [TableVisible]
        [Display(Name = "ID")]
        [Key]
        [ReadOnly(true)]
        public int Id { get; set; }

        [Display(Name = "名称")]
        [TableVisible]
        [ReadOnly(true)]
        public string Name { get; set; }

        [Display(Name = "帮助")]
        [TableVisible]
        [ReadOnly(true)]
        public string Help { get; set; }

        [Display(Name="广告图")]
        [TableRows]
        public ImageItem[] Items { get; set; }
    }
}

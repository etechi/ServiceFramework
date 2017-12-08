using System.ComponentModel.DataAnnotations;

namespace SF.Auth.IdentityServices.Externals
{
	public enum ExtAuthType
	{
		OAuth2=0
	}
    [EntityObject("外部认证描述")]
    public class ExtAuthDescriptor : 
		IObjectWithId<string>
	{
		[Key]
		[StringLength(20)]
		[Display(Name="外部认证标示",Description ="注意必须和服务提供者标示保持一致")]
		[Required]
		[TableVisible]
		public string Id { get; set; }

		[Display(Name = "外部认证名称")]
		[TableVisible]
		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[Display(Name = "外部认证图标")]
		[Image]
		public string Icon { get; set; }

		[Display(Name = "外部认证协议类型")]
		[TableVisible]
		[Required]
		public ExtAuthType Type { get; set; }

		[Display(Name="标示符类型", Description = "用户标示符类型，外部认证开始使用后，不能在调整")]
		[TableVisible]
		[Required]
		[Range(30,100)]
		public byte IdentType{get;set;}



		[Display(Name = "排位")]
		[TableVisible]
		[Required]
		public int Order { get; set; }


		[Display(Name = "对象状态")]
		[TableVisible]
		[Required]
		public LogicObjectState ObjectState { get; set; }



	}
}

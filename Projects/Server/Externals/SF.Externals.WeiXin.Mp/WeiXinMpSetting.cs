namespace SF.Externals.WeiXin.Mp
{
	/// <summary>
	/// 微信公众平台通用设置
	/// </summary>
    public class WeiXinMpSetting
    {
		///<title>公众号APPID</title>
		/// <summary>
		/// 公众号唯一标识
		/// </summary>
        public string AppId { get; set; } 

		/// <summary>
		/// 公众号APPSecret
		/// </summary>
        public string AppSecret { get; set; }

		/// <summary>
		/// 微信接口基地址
		/// </summary>
		public string ApiUriBase { get; set; } = "https://api.weixin.qq.com/cgi-bin/";
    }
}

using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Payments.Platforms.Weichat
{

    //[Setting(Name = "微信支付",Group ="外部接口")]
    public class WeiChatPaySetting 
	{
        ///<title>公众号APPID</title>
        /// <summary>
        /// 公众号唯一标识
        /// </summary>
		[Required]
		[MaxLength(100)]
		public string AppId { get; set; }

        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        ///<title>微信支付商户号</title>
        /// <summary>
        /// 微信支付商户号
        /// </summary>
		[Required]
		[MaxLength(100)]
		public string MCHID { get; set; }

        ///<title>商户支付支付秘钥</title>
        /// <summary>
        /// 商户支付密钥，参考开户邮件设置
        /// </summary>
		[Required]
		[MaxLength(100)]
		public string KEY { get; set; } 
        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        */
        ///<title>证书路径</title>
        /// <summary>
        /// 证书路径,注意填写相对于网站根目录的路径（仅退款、撤销订单时需要）
        /// </summary>
		[Required]
		[MaxLength(100)]
		public string SSLCERT_PATH { get; set; }

        ///<title>证书密码</title>
        /// <summary>
        /// 仅退款、撤销订单时需要
        /// </summary>
		[Required]
		[MaxLength(100)]
		public string SSLCERT_PASSWORD { get; set; }


	}
}

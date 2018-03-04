using SF.Common.Notifications;
using SF.Sys;
using SF.Sys.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SF.Sys.Threading;
using SF.Common.Notifications.Senders;
using System.Collections.Generic;
using SF.Sys.Linq;
using SF.Externals.WeiXin.Mp;
using System.ComponentModel.DataAnnotations;

namespace SF.Externals.WeiXin.Devices
{
	public class WeiXinDeviceServiceSetting
	{
		/// <summary>
		/// 微信设备接口基地址
		/// </summary>
		[Required]
		public string ApiUriBase { get; set; } = "https://api.weixin.qq.com/device/";
	}
	/// <summary>
	/// 微信设备服务
	/// </summary>
	public class WeiXinDeviceService : IWeiXinDeviceService
	{
        public IAccessTokenManager AccessTokenManager { get; }
		public IUserProfileService UserProfileService { get; }
		public WeiXinDeviceServiceSetting Setting { get; }
		async Task<string> GetUserOpenId (long UserId)
		{
			var re = await UserProfileService.GetClaims(UserId, new[] { PredefinedClaimTypes.WeiXinMPId }, null);
			var id = re.FirstOrDefault();
			if (id == null)
				throw new PublicInvalidOperationException("用户未绑定微信服务号");
			return id.Value;
		}
		public WeiXinDeviceService(
			IAccessTokenManager AccessTokenManager,
			IUserProfileService UserProfileService,
			WeiXinDeviceServiceSetting Setting
			)
		{
			this.AccessTokenManager = AccessTokenManager;
			this.UserProfileService = UserProfileService;
			this.Setting = Setting;
		}

		interface IRespStatus
		{
			int errcode { get; }
			string errmsg { get; }
		}

		class getqrcode_resp_status : IRespStatus
		{
			public int ret_code { get; set; }
			public string error_info { get; set; }

			int IRespStatus.errcode => ret_code;

			string IRespStatus.errmsg => error_info;
		}
		class getqrcode_resp
		{
			public getqrcode_resp_status resp_msg { get; set; }
			public string deviceid { get; set; }
			public string qrticket { get; set; }
		}
		void ValidateResponse(IRespStatus status,string op)
		{
			if ((status?.errcode??0) != 0)
				throw new WeiXinException(status.errcode, $"{op}操作失败:{status.errcode}:{status.errmsg}");
		}

		public async Task<GetQrCodeResult> GetQrCode(string ProductId)
		{
			var re = await AccessTokenManager.RequestString(
				new Uri(Setting.ApiUriBase + "getqrcode")
				.WithQueryString(("product_id", ProductId)),
				null
				);
			var resp = Json.Parse<getqrcode_resp>(re);
			ValidateResponse(resp.resp_msg, "设备授权");
			if (resp.deviceid.IsNullOrEmpty())
				throw new WeiXinException(-1, "请求成功，但没有返回设备ID");
			var id = resp.deviceid.LastSplit2('_').Item2;
			return new GetQrCodeResult
			{
				DeviceId = id,
				QrCode = resp.qrticket
			};
		}

		class resp_status : IRespStatus
		{
			public int errcode { get; set; }
			public string errmsg { get; set; }
		}
		class auth_resp : resp_status
		{
			public resp_status[] resp { get; set; }
		}
		public async Task AuthorizeDevice(AuthorizeDeviceArgument Arg)
		{
			var devs = new[]{
				new
				{
					id = Arg.DeviceId,
					mac = Arg.Mac,
					connect_protocol = Arg.ConnectProtocols.Select(p => (int)p).Join("|"),
					auth_key = Arg.AuthKey,
					close_strategy = ((int)Arg.CloseStrategy).ToString(),
					conn_strategy = ((Arg.AutoReconnectOnPage ? 1 : 0) | (Arg.AutoRecordOnActive ? 4 : 0)).ToString(),
					crypt_method = ((int)Arg.CryptMethod).ToString(),
					auth_ver = Arg.AuthVersion,
					manu_mac_pos = Arg.ManuMacPos.ToString(),
					ser_mac_pos = Arg.SerMacPos.ToString(),
					ble_simple_protocol = Arg.BleSimpleProtocol
				}
			};
			var re = await AccessTokenManager.Json(
				new Uri(Setting.ApiUriBase + "authorize_device"),
				new
				{
					device_num = 1,
					device_list =devs,
					op_type = "0",
					product_id=Arg.ProductId
				}
				);
			
			var result = Json.Parse<auth_resp>(re);
			if(result.errcode== 100002)
			{
				re = await AccessTokenManager.Json(
				   new Uri(Setting.ApiUriBase + "authorize_device"),
				   new
				   {
					   device_num = 1,
					   device_list = devs,
					   op_type = "1",
					   product_id = Arg.ProductId
				   }
				   );
				result = Json.Parse<auth_resp>(re);
			}
			ValidateResponse(result, "设备授权");

			if (result.resp == null || result.resp.Length != 1)
				throw new WeiXinException(-1, $"设备授权失败:没有返回数据");
			ValidateResponse(result.resp[0], "设备授权");
		}
		class bind_resp
		{
			public resp_status base_resp { get; set; }
		}
		public async Task BindDevice(string DeviceId, long UserId, string Ticket)
		{
			var openId = await GetUserOpenId(UserId);

			var re = await AccessTokenManager.Json(
				new Uri(Setting.ApiUriBase +(Ticket==null? "compel_bind" : "bind")),
				new
				{
					ticket=Ticket,
					device_id=DeviceId,
					openid=openId
				}
				);
			var result = Json.Parse<bind_resp>(re);
			ValidateResponse(result.base_resp,"绑定设备");
		}

		public async Task UnbindDevice(string DeviceId, long UserId, string Ticket)
		{
			var openId = await GetUserOpenId(UserId);

			var re = await AccessTokenManager.Json(
				new Uri(Setting.ApiUriBase + (Ticket == null ? "compel_unbind" : "unbind")),
				new
				{
					ticket = Ticket,
					device_id = DeviceId,
					openid = openId
				}
				);
			var result = Json.Parse<bind_resp>(re);
			ValidateResponse(result.base_resp, "解绑设备");
		}
		class dev_status_resp : resp_status
		{
			public int status { get; set; }
			public string status_info { get; set; }
		}
		public async Task<DeviceStatus> GetDeviceStatus(string DeviceId)
		{
			var re = await AccessTokenManager.RequestString(
				new Uri(Setting.ApiUriBase + "get_stat")
				.WithQueryString(("device_id", DeviceId)),
				null
				);
			var resp = Json.Parse<dev_status_resp>(re);
			ValidateResponse(resp, "获取设备状态");
			switch (resp.status)
			{
				case 0:
					return DeviceStatus.Unauth;
				case 1:
					return DeviceStatus.Auth;
				case 2:
				case 3:
					return DeviceStatus.Binded;
				default:
					throw new WeiXinException(-1, $"不支持的设备状态:{resp.status}:{resp.status_info}");
			}
		}

		
	}
}

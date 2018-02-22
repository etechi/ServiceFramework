using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Devices
{
	public class GetQrCodeResult
	{
		public string DeviceId { get; set; }
		public string QrCode { get; set; }
	}
	public enum ConnectProtocol {
		AndroidClassicBluetooth = 1,
		IOSClassicBluetooth = 2,
		BLE = 3,
		Wifi = 4
	}
	public enum CloseStrategy
	{
		/// <summary>
		/// 1：退出公众号页面时即断开连接 
		/// </summary>
		DisconnectAfterExit,

		/// <summary>
		/// 2：退出公众号之后保持连接不断开
		/// </summary>
		UndisconnectAfterExit
	}

	public enum CryptMethod {
		/// <summary>
		/// 不加密
		/// </summary>
		Uncrypt = 0,
		/// <summary>
		/// AES加密（CBC模式，PKCS7填充方式） 
		/// </summary>
		AES = 1
	}

	public class AuthorizeDeviceArgument
	{
		/// <summary>
		/// 是,设备的deviceid
		/// </summary>
		public string DeviceId { get; set; }
		
		/// <summary>
		/// 是,设备的mac地址 格式采用16进制串的方式（长度为12字节），不需要0X前缀，如： 1234567890AB
		/// </summary>
		public string Mac { get; set; }

		/// <summary>
		/// 连接协议
		/// </summary>
		public ConnectProtocol[] ConnectProtocols { get; set; }

		/// <summary>
		/// auth及通信的加密key，第三方需要将key烧制在设备上（128bit），格式采用16进制串的方式（长度为32字节），不需要0X前缀，如： 1234567890ABCDEF1234567890ABCDEF
		/// </summary>
		public string AuthKey { get; set; }

		/// <summary>
		/// 断开策略，目前支持： 1：退出公众号页面时即断开连接 2：退出公众号之后保持连接不断开
		/// </summary>

		public CloseStrategy CloseStrategy { get; set; }


		/// <summary>
		/// 1：（第1bit置位）在公众号对话页面，不停的尝试连接设备 
		/// </summary>
		public bool AutoReconnectOnPage { get; set; }

		/// <summary>
		/// 4：（第3bit置位）处于非公众号页面（如主界面等），微信自动连接。当用户切换微信到前台时，可能尝试去连接设备，连上后一定时间会断开
		/// </summary>
		public bool AutoRecordOnActive { get; set; }

		/// <summary>
		/// 加密方式
		/// </summary>
		public CryptMethod CryptMethod { get; set; }

		/// <summary>
		/// 认证版本
		/// </summary>
		public string AuthVersion { get; set; }

		/// <summary>
		/// 表示mac地址在厂商广播manufature data里含有mac地址的偏移，取值如下： -1：在尾部、 -2：表示不包含mac地址 其他：非法偏移
		/// </summary>
		public int ManuMacPos { get; set; }

		/// <summary>
		/// 表示mac地址在厂商serial number里含有mac地址的偏移，取值如下： -1：表示在尾部 -2：表示不包含mac地址 其他：非法偏移
		/// </summary>
		public int SerMacPos { get; set; }

		public string BleSimpleProtocol { get; set; }
	}
	public enum DeviceStatus
	{
		/// <summary>
		/// 未授权
		/// </summary>
		Unauth,
		/// <summary>
		/// 已授权
		/// </summary>
		Auth,
		/// <summary>
		/// 已绑定
		/// </summary>
		Binded
	}
    public interface IWeiXinDeviceService
    {

        Task<GetQrCodeResult> GetQrCode(string ProductId);
		Task AuthorizeDevice(AuthorizeDeviceArgument Arg);
		Task BindDevice(string DeviceId, long UserId, string Ticket);
		Task UnbindDevice(string DeviceId, long UserId, string Ticket);
		Task<DeviceStatus> GetDeviceStatus(string DeviceId);
	}
}

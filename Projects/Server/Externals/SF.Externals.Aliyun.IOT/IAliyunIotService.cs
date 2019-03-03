using SF.Common.Notifications.Senders;
using SF.Sys.Auth;
using SF.Sys.Collections.Generic;
using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Externals.Aliyun
{
	public enum RrpcCode
	{
		UNKNOW,
		SUCCESS,
		TIMEOUT,
		OFFLINE,
		HALFCONN,
        UNKONW //阿里云有时候返回这个错误码
    }
	public enum DeviceStatus
	{
		ONLINE,
		OFFLINE,
		UNACTIVE
	}
	public class RrpcArgument
	{
		public string ProductKey { get; set; }
		public string DeviceName { get; set; }
		public byte[] RequestBase64Byte { get; set; }
		public int Timeout { get; set; }
	}
	
	public class DeviceNameAndStatus
	{
		public string DeviceName { get; set; }
		public DeviceStatus Status { get; set; }
	}
	public class RrpcException : AliyunIOTException
	{
		public RrpcException(string Code, string Message, string RequestId, string HostId) : 
			base(Code, Message, RequestId, HostId)
		{
		}

		public string MessageId { get; set; }
		public bool Success { get; set; }
		public RrpcCode RrpcCode { get; set; }
	}

	public class Message
	{
		public string ProductKey { get; set; }
		public string TopicFullName { get; set; }
		public byte[] MessageContent { get; set; }
		public int Qos { get; set; }
	}
	public class DeviceInfo
	{
		public string DeviceId { get; set; }
		public string DeviceName { get; set; }
		public string ProductKey { get; set; }
		public string DeviceSecret { get; set; }
		public DateTime GmtCreate { get; set; }
		public DateTime GmtModified { get; set; }
	}
	
	public class DeviceShadowItem
	{
		public string Key { get; set; }
		public string Value { get; set; }
		public int Timestamp { get; set; }
	}
	public class DeviceShadowReply
	{
		public int Version { get; set; }
		public int Timestamp { get; set; }
		public Dictionary<string, DeviceShadowItem> Desired { get; set; }
		public Dictionary<string, DeviceShadowItem> Reported { get; set; }
	}
	public class DeviceShadowUpdateArgument
	{
		public string ProductKey { get; set; }
		public string DeviceName { get; set; }
		public int Version { get; set; }
		public Dictionary<string, string> Desired { get; set; }
		public Dictionary<string, string> Reported { get; set; }
	}
	public class DeviceShadowRemoveArgument
	{
		public string ProductKey { get; set; }
		public string DeviceName { get; set; }
		public int Version { get; set; }
		public string[] Desired { get; set; }
		public string[] Reported { get; set; }
	}
	public class AliyunIOTException : AliyunException
	{
		public AliyunIOTException(
			string Code, 
			string Message, 
			string RequestId, 
			string HostId
			) : base(Code, Message, RequestId, HostId)
		{
		}
	}
	/// <summary>
	/// 阿里云IOT服务
	/// </summary>
	public interface IAliyunIOTService
	{
		/// <summary>
		/// RRPC调用
		/// </summary>
		/// <param name="Arg"></param>
		/// <returns>设备应答报文</returns>
		Task<byte[]> RrpcCall(RrpcArgument Arg);


		Task<string> SendMessage(Message Message);

		Task<DeviceNameAndStatus[]> BatchGetDeviceState(string ProductKey, IEnumerable<string> DeviceNames);
		//Task<int> ApplyDevices(string ProductKey, string DeviceName);

		Task<DeviceInfo> QueryDeviceByName(string ProductKey, string DeviceName);

		Task<DeviceInfo> RegistDevice(string ProductKey, string DeviceName);

		Task<DeviceShadowReply> DeviceShadowGet(string ProductKey, string DeviceName);
		Task DeviceShadowUpdate(DeviceShadowUpdateArgument Argument);
		Task DeviceShadowRemove(DeviceShadowRemoveArgument Arg);
	}
}

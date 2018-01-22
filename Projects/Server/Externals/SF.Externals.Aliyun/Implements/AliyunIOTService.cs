using SF.Sys;
using SF.Sys.HttpClients;
using SF.Sys.Linq;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SF.Externals.Aliyun.Implements
{
	public class AliyunIOTSetting
	{
		///<title>IOT接口服务域名</title>
		/// <summary>
		/// 如： https://iot.cn-shanghai.aliyuncs.com/
		/// </summary>
		public string Uri { get; set; }

		///<title> 区域ID</title>
		/// <summary>
		/// 如：cn-shanghai
		/// </summary>
		public string RegionId { get; set; }

		///<title>API版本</title>
		/// <summary>
		/// yyyy-MM-dd 格式，如:2017-04-20
		/// </summary>
		public string ApiVersion { get; set; }
	}
	public class AliyunIOTService : IAliyunIOTService
	{
		public IAliyunInvoker AliyunInvoker { get; }
		public AliyunIOTSetting IOTSetting { get; }
		public AliyunIOTService(AliyunIOTSetting IOTSetting,IAliyunInvoker AliyunInvoker)
		{
			this.IOTSetting = IOTSetting;
			this.AliyunInvoker = AliyunInvoker;
		}
		(string Key, string Value)[] GetApiSettings()
		{
			return new[] {
				("RegionId", IOTSetting.RegionId),
				("Version", IOTSetting.ApiVersion)
			};
		}

		class QueryDeviceByNameResult
		{
			public string RequestId { get; set; }
			public bool Success { get; set; }
			public DeviceInfo DeviceInfo { get; set; }
		}
		public async Task<DeviceInfo> QueryDeviceByName(string ProductKey, string DeviceName)
		{
			var re = await AliyunInvoker.Invoke(
				"GET",
				IOTSetting.Uri,
				GetApiSettings(),
				("Action", "QueryDeviceByName"),
				(nameof(ProductKey), ProductKey),
				(nameof(DeviceName), DeviceName)
				);
			var result = Json.Parse<QueryDeviceByNameResult>(re);

			if (!result.Success)
				throw new AliyunException(
					"UNKNOWN",
					"根据名称查询设备信息异常,查询失败",
					result.RequestId,
					IOTSetting.Uri
					);
			if (result.DeviceInfo == null)
				return null;
			if(result.DeviceInfo.ProductKey != ProductKey)
				throw new AliyunException(
					"UNKNOWN",
					$"根据名称查询设备信息异常,ProductKey不一致，期望:{ProductKey},实际:{result.DeviceInfo.ProductKey}",
					result.RequestId,
					IOTSetting.Uri
					);

			if (result.DeviceInfo.DeviceName != DeviceName)
				throw new AliyunException(
					"UNKNOWN",
					$"根据名称查询设备信息异常,DeviceName不一致，期望:{DeviceName},实际:{result.DeviceInfo.DeviceName}",
					result.RequestId,
					IOTSetting.Uri
					);
			return result.DeviceInfo;
		}

		class DeviceStatusList
		{
			public DeviceNameAndStatus[] DeviceStatus { get; set; }
		}
		class QueryDeviceStatusResult
		{
			public DeviceStatusList DeviceStatusList { get; set; }
			public string RequestId { get; set; }
			public bool Success { get; set; }
		}
		public async Task<DeviceNameAndStatus[]> BatchGetDeviceState(
			string ProductKey, 
			IEnumerable<string> DeviceNames
			)
		{
			var req = new List<(string Key, string Value)>
			{
				("Action","BatchGetDeviceState"),
				(nameof(ProductKey),ProductKey)
			};
			req.AddRange(
				DeviceNames.Select((n, i) => ("DeviceName." + (i + 1), n))
				);
			var re = await AliyunInvoker.Invoke(
				"GET",
				IOTSetting.Uri,
				GetApiSettings(),
				req.ToArray()
				);

			var result = Json.Parse<QueryDeviceStatusResult>(re);
			if (!result.Success)
				throw new AliyunException("UNKNOWN", "未知错误",result.RequestId,IOTSetting.Uri);

			return result?.DeviceStatusList?.DeviceStatus ?? Array.Empty<DeviceNameAndStatus>();

		}


		class RrpcResult
		{
			public string RequestId { get; set; }
			public bool Success { get; set; }
			public string MessageId { get; set; }
			public RrpcCode RrpcCode { get; set; }
			public string PayloadBase64Byte { get; set; }
		}
		public async Task<byte[]> RrpcCall(RrpcArgument Arg)
		{
			var re = await AliyunInvoker.Invoke(
				"GET",
				IOTSetting.Uri,
				GetApiSettings(),
				("Action", "RRpc"),
				(nameof(Arg.ProductKey), Arg.ProductKey),
				(nameof(Arg.DeviceName), Arg.DeviceName),
				(nameof(Arg.RequestBase64Byte), Arg.RequestBase64Byte.Base64()),
				(nameof(Arg.Timeout), Arg.Timeout.ToString())
				);
			var result = Json.Parse<RrpcResult>(re);
			if (result.Success && result.RrpcCode==RrpcCode.SUCCESS)
				return result.PayloadBase64Byte.Base64();
			throw new RrpcException("UNKNOWN", "没有返回消息ID", result.RequestId, IOTSetting.Uri)
			{
				MessageId=result.MessageId,
				RrpcCode = result.RrpcCode,
				Success = result.Success
			};
		}

		class SendMessageResult
		{
			public string RequestId { get; set; }
			public bool Success { get; set; }
			public string MessageId { get; set; }
		}	

		public async Task<string> SendMessage(Message Message)
		{
			var re=await AliyunInvoker.Invoke(
				"GET",
				IOTSetting.Uri,
				GetApiSettings(),
				("Action", "Pub"),
				(nameof(Message.ProductKey), Message.ProductKey),
				(nameof(Message.TopicFullName), Message.TopicFullName),
				(nameof(Message.MessageContent), Message.MessageContent.Base64()),
				(nameof(Message.Qos), Message.Qos.ToString())
				);
			var result = Json.Parse<SendMessageResult>(re);
			if (result.MessageId.IsNullOrEmpty())
				throw new AliyunException("UNKNOWN", "没有返回消息ID", result.RequestId, IOTSetting.Uri);
			return result.MessageId;
		}

		
		class SetDeviceShadowResult
		{
			public string RequestId { get; set; }
			public bool Success { get; set; }
			public string ErrorMessage { get; set; }
		}

		public class DeviceShadowState
		{
			public Dictionary<string,string> desired { get; set; }
			public Dictionary<string,string> reported { get; set; }
		}
		public class DeviceShadowTimestamp
		{
			public int timestamp { get; set; }
		}
		public class DeviceShadowMetadata
		{
			public Dictionary<string, DeviceShadowTimestamp> desired { get; set; }
			public Dictionary<string, DeviceShadowTimestamp> reported { get; set; }
		}
		class DeviceShadowMessage
		{
			public string method { get; set; }
			public int version { get; set; }
			public int timestamp { get; set; }
			public DeviceShadowState state { get; set; }
			public DeviceShadowMetadata metadata { get; set; }
		}
		class GetDeviceShadowResult
		{
			public string RequestId { get; set; }
			public bool Success { get; set; }
			public string ErrorMessage { get; set; }
			public string ShadowMessage { get; set; }
		}
		public async Task<DeviceShadowReply> DeviceShadowGet(string ProductKey, string DeviceName)
		{
			var re = await AliyunInvoker.Invoke(
				"GET",
				IOTSetting.Uri,
				GetApiSettings(),
				("Action", "GetDeviceShadow"),
				(nameof(ProductKey), ProductKey),
				(nameof(DeviceName), DeviceName)
				);
			var result = Json.Parse<GetDeviceShadowResult>(re);
			if (!result.Success)
				throw new AliyunException(
					"UNKNOWN",
					"获取设备影子错误:" + result.ErrorMessage,
					result.RequestId,
					IOTSetting.Uri
					);
			var msg = Json.Parse<DeviceShadowMessage>(result.ShadowMessage);
			var reply = new DeviceShadowReply
			{
				Version = msg.version,
				Timestamp = msg.timestamp,
				Desired = msg.state?.desired?.Select(p => new DeviceShadowItem
				{
					Key = p.Key,
					Value = p.Value
				})?.ToDictionary(p => p.Key),
				Reported = msg.state?.reported?.Select(p => new DeviceShadowItem
				{
					Key = p.Key,
					Value = p.Value
				})?.ToDictionary(p => p.Key)
			};
			if (msg.metadata?.reported != null)
				foreach (var p in msg.metadata.reported)
				{
					DeviceShadowItem dsi = null;
					if (reply?.Reported?.TryGetValue(p.Key, out dsi) ?? false)
						dsi.Timestamp = p.Value.timestamp;
				}
			if (msg.metadata?.desired!= null)
				foreach (var p in msg.metadata.desired)
				{
					DeviceShadowItem dsi = null;
					if (reply?.Desired?.TryGetValue(p.Key, out dsi) ?? false)
						dsi.Timestamp = p.Value.timestamp;
				}

			return reply;
		}
		public async Task DeviceShadowUpdate(DeviceShadowUpdateArgument Arg)
		{
			var ShadowMessage = new DeviceShadowMessage
			{
				method = "update",
				version = Arg.Version,
				state = new DeviceShadowState
				{
					desired = Arg.Desired,//?.Select(p=>(Key:p.Key,Value:p.Value ?? "none"))?.ToDictionary(p=>p.Key,p=>p.Value),
					reported = Arg.Reported,//?.Select(p => (Key: p.Key, Value: p.Value ?? "none"))?.ToDictionary(p => p.Key, p => p.Value)
				}
			};


			var re = await AliyunInvoker.Invoke(
				"GET",
				IOTSetting.Uri,
				GetApiSettings(),
				("Action", "UpdateDeviceShadow"),
				(nameof(Arg.ProductKey), Arg.ProductKey),
				(nameof(Arg.DeviceName), Arg.DeviceName),
				(nameof(ShadowMessage), Json.Stringify(ShadowMessage))
				);
			var result = Json.Parse<SetDeviceShadowResult>(re);
			if (result.Success)
				return;

			throw new AliyunException(
				"UNKNOWN",
				"设置设备影子错误:" + result.ErrorMessage,
				result.RequestId,
				IOTSetting.Uri
				);
		}

		public async Task DeviceShadowRemove(DeviceShadowRemoveArgument Arg)
		{
			var ShadowMessage = new DeviceShadowMessage
			{
				method = "delete",
				version = Arg.Version,
				state = new DeviceShadowState
				{
					desired = Arg.Desired?.ToDictionary(d => d, d => "null"),
					reported = Arg.Reported?.ToDictionary(d => d, d => "null")
				}
			};


			var re = await AliyunInvoker.Invoke(
				"GET",
				IOTSetting.Uri,
				GetApiSettings(),
				("Action", "DeleteDeviceShadow"),
				(nameof(Arg.ProductKey), Arg.ProductKey),
				(nameof(Arg.DeviceName), Arg.DeviceName),
				(nameof(ShadowMessage), Json.Stringify(ShadowMessage))
				);
			var result = Json.Parse<SetDeviceShadowResult>(re);
			if (result.Success)
				return;

			throw new AliyunException(
				"UNKNOWN",
				"设置设备影子错误:" + result.ErrorMessage,
				result.RequestId,
				IOTSetting.Uri
				);
		}
	}
}

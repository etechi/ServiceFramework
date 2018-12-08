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
		public string ApiVersion { get; set; } = "2017-04-20";
	}
	/// <summary>
	/// 阿里云IOT服务
	/// </summary>
	public class AliyunIOTService : IAliyunIOTService
	{
		public IAliyunInvoker AliyunInvoker { get; }
		public AliyunIOTSetting Setting { get; }
		public AliyunIOTService(AliyunIOTSetting Setting,IAliyunInvoker AliyunInvoker)
		{
			this.Setting = Setting;
			this.AliyunInvoker = AliyunInvoker;
		}
		(string Key, string Value)[] GetApiSettings()
		{
			return new[] {
				("RegionId", Setting.RegionId),
				("Version", Setting.ApiVersion)
			};
		}
		class BaseResult
		{
			public string RequestId { get; set; }
			public bool Success { get; set; }
			public string ErrorMessage { get; set; }

            public virtual string Code { get; } = null;
		}
		class QueryDeviceByNameResult : BaseResult
		{	
			public DeviceInfo DeviceInfo { get; set; }
		}
		AliyunIOTException IOTError(string Operation,string Message,string RequestId,string Code=null)
		{
            if(Operation=="RRpc")
                return new RrpcException(
                    Code ?? "RRPCERROR",
                    $"{Operation}失败:{Message ?? "未知错误"}",
                    RequestId,
                    Setting.Uri
                    );
            else
                return new AliyunIOTException(
					Code??"IOTERROR",
					$"{Operation}失败:{Message?? "未知错误"}",
					RequestId,
					Setting.Uri
					);
		}
		void ResultValidate(BaseResult Result,string Operation)
		{
			if (!Result.Success)
				throw IOTError( 
					Operation,
					Result.ErrorMessage,
					Result.RequestId,
                    Result.Code
					);
		}
		public async Task<DeviceInfo> QueryDeviceByName(string ProductKey, string DeviceName)
		{
			var action = "QueryDeviceByName";
			var re = await AliyunInvoker.Invoke(
				"GET",
				Setting.Uri,
				GetApiSettings(),
				("Action", action),
				(nameof(ProductKey), ProductKey),
				(nameof(DeviceName), DeviceName)
				);
			var result = Json.Parse<QueryDeviceByNameResult>(re);
			ResultValidate(result, action);

			if (result.DeviceInfo == null)
				return null;
			if(result.DeviceInfo.ProductKey != ProductKey)
				throw IOTError(
					action,
					$"根据名称查询设备信息异常,ProductKey不一致，期望:{ProductKey},实际:{result.DeviceInfo.ProductKey}",
					result.RequestId,
					Setting.Uri
					);

			if (result.DeviceInfo.DeviceName != DeviceName)
				throw IOTError(
					action,
					$"根据名称查询设备信息异常,DeviceName不一致，期望:{DeviceName},实际:{result.DeviceInfo.DeviceName}",
					result.RequestId
					);
			return result.DeviceInfo;
		}

		class RegistResult : BaseResult
		{
			public string DeviceId { get; set; }
			public string DeviceName { get; set; }
			public string DeviceSecret { get; set; }
			public string DeviceStatus { get; set; }
		}
		public async Task<DeviceInfo> RegistDevice(string ProductKey, string DeviceName)
		{
			var action = "RegistDevice";
			var re = await AliyunInvoker.Invoke(
				"GET",
				Setting.Uri,
				GetApiSettings(),
				("Action", action),
				(nameof(ProductKey), ProductKey),
				(nameof(DeviceName), DeviceName)
				);
			var result = Json.Parse<RegistResult>(re);
			ResultValidate(result, action);

			
			if (result.DeviceName != DeviceName)
				throw IOTError(
					action,
					$"注册设备返回信息异常,DeviceName不一致，期望:{DeviceName},实际:{result.DeviceName}",
					result.RequestId
					);
			return new DeviceInfo
			{
				DeviceId = result.DeviceId,
				DeviceName = result.DeviceName,
				DeviceSecret = result.DeviceSecret,
				ProductKey = ProductKey
			};
		}

		class DeviceStatusList
		{
			public DeviceNameAndStatus[] DeviceStatus { get; set; }
		}
		class QueryDeviceStatusResult :BaseResult
		{
			public DeviceStatusList DeviceStatusList { get; set; }
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
				Setting.Uri,
				GetApiSettings(),
				req.ToArray()
				);

			var result = Json.Parse<QueryDeviceStatusResult>(re);
			ResultValidate(result, "BatchGetDeviceState");

			return result?.DeviceStatusList?.DeviceStatus ?? Array.Empty<DeviceNameAndStatus>();

		}


		class RrpcResult : BaseResult
		{
			public string MessageId { get; set; }
			public RrpcCode RrpcCode { get; set; }
			public string PayloadBase64Byte { get; set; }

            public override string Code => RrpcCode.ToString();
        }
		public async Task<byte[]> RrpcCall(RrpcArgument Arg)
		{
			var re = await AliyunInvoker.Invoke(
				"GET",
				Setting.Uri,
				GetApiSettings(),
				("Action", "RRpc"),
				(nameof(Arg.ProductKey), Arg.ProductKey),
				(nameof(Arg.DeviceName), Arg.DeviceName),
				(nameof(Arg.RequestBase64Byte), Arg.RequestBase64Byte.Base64()),
				(nameof(Arg.Timeout), Arg.Timeout.ToString())
				);
			var result = Json.Parse<RrpcResult>(re);
			ResultValidate(result, "RRpc");
			if (result.RrpcCode==RrpcCode.SUCCESS)
				return result.PayloadBase64Byte.Base64();
			throw new RrpcException("IOTRRPCERROR", "RRpc失败:设备状态无效:" + result.RrpcCode, result.RequestId, Setting.Uri)
			{
				MessageId=result.MessageId,
				RrpcCode = result.RrpcCode,
				Success = result.Success
			};
		}

		class SendMessageResult :BaseResult
		{
			public string MessageId { get; set; }
		}	

		public async Task<string> SendMessage(Message Message)
		{
			var re=await AliyunInvoker.Invoke(
				"GET",
				Setting.Uri,
				GetApiSettings(),
				("Action", "Pub"),
				(nameof(Message.ProductKey), Message.ProductKey),
				(nameof(Message.TopicFullName), Message.TopicFullName),
				(nameof(Message.MessageContent), Message.MessageContent.Base64()),
				(nameof(Message.Qos), Message.Qos.ToString())
				);
			var result = Json.Parse<SendMessageResult>(re);
			ResultValidate(result, "Pub");
			if (result.MessageId.IsNullOrEmpty())
				throw IOTError("Pub", "没有返回消息ID",result.MessageId);
			return result.MessageId;
		}

		
		class SetDeviceShadowResult : BaseResult
		{
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
		class GetDeviceShadowResult :BaseResult
		{
			public string ShadowMessage { get; set; }
		}
		public async Task<DeviceShadowReply> DeviceShadowGet(string ProductKey, string DeviceName)
		{
			var re = await AliyunInvoker.Invoke(
				"GET",
				Setting.Uri,
				GetApiSettings(),
				("Action", "GetDeviceShadow"),
				(nameof(ProductKey), ProductKey),
				(nameof(DeviceName), DeviceName)
				);
			var result = Json.Parse<GetDeviceShadowResult>(re);
			ResultValidate(result, "GetDeviceShadow");

			if (result.ShadowMessage == null)
				return new DeviceShadowReply();
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
				Setting.Uri,
				GetApiSettings(),
				("Action", "UpdateDeviceShadow"),
				(nameof(Arg.ProductKey), Arg.ProductKey),
				(nameof(Arg.DeviceName), Arg.DeviceName),
				(nameof(ShadowMessage), Json.Stringify(ShadowMessage))
				);
			var result = Json.Parse<SetDeviceShadowResult>(re);
			ResultValidate(result, "UpdateDeviceShadow");
		
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
				Setting.Uri,
				GetApiSettings(),
				("Action", "DeleteDeviceShadow"),
				(nameof(Arg.ProductKey), Arg.ProductKey),
				(nameof(Arg.DeviceName), Arg.DeviceName),
				(nameof(ShadowMessage), Json.Stringify(ShadowMessage))
				);
			var result = Json.Parse<SetDeviceShadowResult>(re);
			ResultValidate(result, "DeleteDeviceShadow");

		}

	}
}

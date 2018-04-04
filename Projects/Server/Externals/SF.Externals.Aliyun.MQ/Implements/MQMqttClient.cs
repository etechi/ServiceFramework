using SF.Net;
using SF.Net.Mqtt.Clients;
using SF.Net.Mqtt.Core;
using SF.Sys.Logging;
using SF.Sys.Services;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SF.Externals.Aliyun.Implements
{
	public static class MQMqttClient
	{
		static string MQHmacSha1(string input, string key)
		{
			using (var myhmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
			{
				byte[] byteArray = Encoding.ASCII.GetBytes(input);
				var stream = new MemoryStream(byteArray);
				return Convert.ToBase64String(myhmacsha1.ComputeHash(stream));
			}
		}
		class MqttLogger : Net.Mqtt.Core.ILogger
		{
			public SF.Sys.Logging.ILogger Logger { get; set; }

			public void Write(Net.Mqtt.Core.LogLevel Level, PacketDirect Direct, IPacket Packet, string Message, Exception Exception)
			{
				if (Exception == null)
					Logger.Trace(
						"MQ {0} {1} {2} {3}",
						(Level == Net.Mqtt.Core.LogLevel.Error ? "E" :
						Level == Net.Mqtt.Core.LogLevel.Info ? "I" :
						"T"),
						Direct == PacketDirect.Inbound ? "<" :
						Direct == PacketDirect.Undefined ? "!" : ">",
						Packet,
						Message
						);
				else
					Logger.Error(
						Exception,
						"MQ {0} {1} {2} {3}",
						(Level == Net.Mqtt.Core.LogLevel.Error ? "E" :
						Level == Net.Mqtt.Core.LogLevel.Info ? "I" :
						"T"),
						Direct == PacketDirect.Inbound ? "<" :
						Direct == PacketDirect.Undefined ? "!" : ">",
						Packet,
						Message
						);
			}
		}
		public static SF.Net.Mqtt.Clients.Client Create(
			AliyunMQMqttClientSetting Setting,
			SF.Sys.Logging.ILogger Logger
			)
		{
			var timestamp = ((int)DateTime.Now.TimeOfDay.TotalSeconds).ToString();

			var mqttClientId = $"{Setting.MQGroupId}@@@{Setting.MqttClientId}";
			var mqttUsername = Setting.AliyunAccessKey;
			var mqttPassword = MQHmacSha1(Setting.MQGroupId, Setting.AliyunAccessKeySecret);


			var cc = new SF.Net.Mqtt.Clients.Client();
			cc.Logger = new MqttLogger { Logger = Logger };
			cc.Connector = Net.Connector.For(Setting.MqttServerEndPoint)
				.Tcp(new Net.Sockets.SocketSetting
					{
						ConnectTimeout=5000
					})
					.Mqtt(cc.Logger);
			cc.UserName = mqttUsername;
			cc.Password = mqttPassword;
			cc.ClientIdentifier = mqttClientId;
			return cc;
			
		}
	}
}

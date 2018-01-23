using SF.Net;
using SF.Net.Mqtt.Clients;
using SF.Sys;
using SF.Sys.HttpClients;
using SF.Sys.Linq;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
		public static SF.Net.Mqtt.Clients.Client Create(AliyunMQMqttClientSetting Setting)
		{
			var timestamp = ((int)DateTime.Now.TimeOfDay.TotalSeconds).ToString();

			var mqttClientId = $"{Setting.MQGroupId}@@@{Setting.MqttClientId}";
			var mqttUsername = Setting.AliyunAccessKey;
			var mqttPassword = MQHmacSha1(Setting.MQGroupId, Setting.AliyunAccessKeySecret);


			var cc = new SF.Net.Mqtt.Clients.Client();
			//cc.Logger = new Logger("MQ ");
			cc.Connector = Net.Connector.For(Setting.MqttServerEndPoint).Tcp().Mqtt(cc.Logger);
			cc.UserName = mqttUsername;
			cc.Password = mqttPassword;
			cc.ClientIdentifier = mqttClientId;
			return cc;
			
		}
	}
}

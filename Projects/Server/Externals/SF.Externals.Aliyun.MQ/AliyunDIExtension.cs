using SF.Common.Notifications.Senders;
using SF.Externals.Aliyun;
using SF.Externals.Aliyun.Implements;
using System;
using System.Threading.Tasks;
using SF.Sys.Threading;
using SF.Sys.Settings;

namespace SF.Sys.Services
{
	public class AliyunMQMqttClientSetting
	{
		/// <summary>
		/// 阿里云APPID
		/// </summary>
		public string AliyunAccessKey { get; set; }
		/// <summary>
		/// 阿里云访问密钥
		/// </summary>
		public string AliyunAccessKeySecret { get; set; }
		/// <summary>
		/// 消息队列GroupId
		/// </summary>
		public string MQGroupId { get; set; }
		///<title>Mqtt服务端口</title>
		/// <summary>
		/// Ex:post-cn-4590fdcmd02.mqtt.aliyuncs.com:1883
		/// </summary>
		public string MqttServerEndPoint { get; set; }
		/// <summary>
		/// 阿里云客户端ID
		/// </summary>
		public string MqttClientId { get; set; }
	}
	public class AddAliyunMQMqttClientArgument<TSetting> where TSetting:AliyunMQMqttClientSetting
	{
		public TSetting Setting { get; set; }
		public string ServiceName { get; set; }
		public Func<IServiceProvider,Task> Init { get; set; }
		public bool AutoStartup { get; set; } = true;
		public Func<IServiceProvider, SF.Net.Mqtt.Clients.Client, TSetting, Task> OnClientCreated { get; set; }
	}
	public static class AliyunDIExtension
    {
        public static IServiceCollection AddAliyunMQMqttClientService<TSetting>(
			this IServiceCollection sc,
			AddAliyunMQMqttClientArgument<TSetting> Arg
			) where TSetting: AliyunMQMqttClientSetting,new()
		{
			sc.AddSetting(Arg.Setting);
			sc.AddTaskService(
				Arg.ServiceName,
				Arg.Init,
				async (sp, state, ct) =>
				{
					while (!ct.IsCancellationRequested)
					{
						using (var ict = new System.Threading.CancellationTokenSource())
						{
							using (ct.Register(() => ict.Cancel()))
							{
								var scts = sp.Resolve<ISettingChangedTrackerService>();
								using (scts.OnSettingChanged<TSetting>(
									isp =>
									{
										ict.Cancel();
										return Task.CompletedTask;
									}))
								{

									var cliSetting = sp.Resolve<ISettingService<TSetting>>().Value;
									using (var cli = MQMqttClient.Create(cliSetting))
									{
										await Arg.OnClientCreated(sp, cli,cliSetting);
										cli.Start();
										await ict.Token.WaitAsync();
									}
								}
							}
						}
					}
				},
				Arg.AutoStartup
				);
			return sc;
		}
        
    }
}

using SF.Sys.Linq;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.TextMessages
{
	//public static class MsgServiceTypes
 //   {
 //       public static readonly string 邮件 = "邮件";
 //       public static readonly string APP推送 = "APP推送";
 //       public static readonly string 验证短信 = "验证短信";
 //       public static readonly string 通知短信 = "通知短信";
 //       public static readonly string 促销短信 = "促销短信";
 //       public static readonly string 微信通知 = "微信通知";
 //   }
 //   [RequireServiceType(typeof(IMsgProvider), "邮件", "APP推送","验证短信","通知短信","促销短信", "微信通知")]
    public class MsgService : IMsgService
	{
		public MsgServiceSetting Setting { get; }
		public IServiceProvider ServiceProvider { get; }

		public IMsgLogger Logger { get; }
		public MsgService(MsgServiceSetting Setting, IServiceProvider ServiceProvider, IMsgLogger Logger)
		{
			this.Setting = Setting;
			this.ServiceProvider = ServiceProvider;
			this.Logger = Logger;
		}

		bool IsDisabled()
		{
            return Setting.Disabled;
			
		}
		async Task<MessageSendResult> Send(IMsgProvider provider, string address, Message message)
		{
			try
			{
				var re = await provider.Send(address, message);
				return new MessageSendResult(address, re, null);
			}
			catch(Exception e)
			{
				return new MessageSendResult(address, null, e);
			}
		}
		static string[] GetResults(string[] addresses, IEnumerable<MessageSendResult> results)
		{
			if (addresses.Length == 1)
				return new[] { results.First().Result };
			
			var dict = results.ToDictionary(r => r.Target);
			return addresses.Select(a => dict[a].Result).ToArray();
		}
		async Task<string[]> SingleSend(
            object context,
            IMsgProvider provider, 
            string[] addresses, 
            Message message
            )
		{
			var re = new List<MessageSendResult>();
			foreach (var a in addresses)
				re.Add(await Send(provider, a, message));
			try
			{
				await this.Logger.PostSend(context,re, null);
			}
			catch { }
			if (re.Any(r => r.Exception != null))
				throw new MessageSendFailedException("消息发送失败", re.ToArray(), null);
			return GetResults(addresses, re);
		}
		async Task<string[]> BatchSend(object context,IMsgBatchProvider batchProvider,string[] addresses,Message message)
		{
			try
			{
				var re = await batchProvider.Send(addresses, message);
				try
				{
					await this.Logger.PostSend(context,re, null);
				}
				catch { }
				return GetResults(addresses, re);
			}
			catch (MessageSendFailedException e)
			{
				try
				{
					await this.Logger.PostSend(context, e.Results, null);
				}
				catch { }
				throw;
			}
			catch (Exception e)
			{
				try
				{
					await this.Logger.PostSend(context, null, e);
				}
				catch { }
				throw;
			}
		}
		async Task<string[]> SendInternal(IServiceInstanceDescriptor Service,IMsgProvider provider,long? targetUserId, string[] addresses, Message message)
		{
            object context = null;
			if (this.Logger != null)
				context=await this.Logger.PreSend(Service, message, targetUserId, addresses);
			
			var bp = provider as IMsgBatchProvider;
			if (bp == null)
				return await SingleSend(context,provider, addresses, message);
			return await BatchSend(context,bp, addresses, message);
		}
		public async Task<string[]> Send(long SysServiceId, string[] addresses, Message message)
		{
			if (IsDisabled())
				return null;
			var resolver = ServiceProvider.Resolver();
			var provider = resolver.Resolve<IMsgProvider>(SysServiceId);
			if (provider == null)
				throw new ArgumentException(
					$"找不到消息提供者：{SysServiceId} 地址:{addresses.Join(";")} 消息：{message} 消息头：{message.Headers?.Select(p=>p.Key+":"+p.Value).Join()}");
            var svc = resolver.ResolveDescriptorByIdent(SysServiceId,typeof(IMsgProvider));

            return await SendInternal(svc,provider, null, addresses, message);
		}
		public async Task<string[]> Send(string SysServiceType, long? TargetUserId, string[] addresses, Message message)
		{
			if (IsDisabled())
				return null;
			var resolver = ServiceProvider.Resolver();
			var provider = (IMsgProvider)resolver.ResolveServiceByType(null,typeof(IMsgProvider),SysServiceType);
			if (provider == null)
				throw new ArgumentException($"找不到默认消息提供者：{SysServiceType} 地址:{string.Join(",", addresses)} 消息：{message} 消息头：{message.Headers?.Select(p => p.Key + ":" + p.Value).Join()}");
			var svc = resolver.ResolveDescriptorByType(null,typeof(IMsgProvider),SysServiceType);
			return await SendInternal(svc, provider, TargetUserId, addresses, message);
		}
		public async Task<string> Send(long SysServiceId, long? TargetUserId,string address, Message message)
		{
			if (IsDisabled())
				return null;
			var resolver = ServiceProvider.Resolver();
			var provider = resolver.Resolve<IMsgProvider>(SysServiceId);
			if (provider == null)
                throw new ArgumentException(
                    $"找不到消息提供者：{SysServiceId} 用户ID:{TargetUserId} 地址:{address} 消息：{message} 消息头：{message.Headers?.Select(p => p.Key + ":" + p.Value).Join()}");
			var svc = resolver.ResolveDescriptorByIdent(SysServiceId, typeof(IMsgProvider));

			var re = await SendInternal(svc, provider, TargetUserId,new []{ address}, message);
            return re.FirstOrDefault();
        }
		public async Task<string> Send(string SysServiceType, long? TargetUserId, string address, Message message)
		{
			if (IsDisabled())
				return null;
			var resolver = ServiceProvider.Resolver();
			var provider = (IMsgProvider)resolver.ResolveServiceByType(null, typeof(IMsgProvider), SysServiceType);
			if (provider == null)
				throw new ArgumentException($"找不到默认消息提供者：{SysServiceType} 用户ID:{TargetUserId} 地址:{address} 消息：{message} 消息头：{message.Headers?.Select(p => p.Key + ":" + p.Value).Join()}");
			var svc = resolver.ResolveDescriptorByType(null, typeof(IMsgProvider), SysServiceType);
			var re = await SendInternal(svc, provider, TargetUserId, new[] { address }, message);
			return re.FirstOrDefault();
		}
	}
}

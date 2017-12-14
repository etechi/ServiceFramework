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
    public class MsgService :ITextMessageService
	{
		public MsgServiceSetting Setting { get; }
		public TypedInstanceResolver<IMsgProvider> MsgProviderResolver { get; }
		public IMsgArgumentFactory ArgumentFactory { get; }
		public IMsgLogger Logger { get; }
		public MsgService(
			MsgServiceSetting Setting, 
			IMsgArgumentFactory ArgumentFactory,
			TypedInstanceResolver<IMsgProvider> MsgProviderResolver, 
			IMsgLogger Logger
			)
		{
			this.Setting = Setting;
			this.MsgProviderResolver = MsgProviderResolver;
			this.ArgumentFactory = ArgumentFactory;
			this.Logger = Logger;
		}

		bool IsDisabled()
		{
            return Setting.Disabled;
			
		}
		public async Task<long> Send(long? targetId, Message message)
		{
			if (IsDisabled())
				return 0;
			return await Logger.Add(
				targetId,
				message,
				async (al) =>
				{
					var re = await ArgumentFactory.Create(targetId, message);

					List<Exception> Errors = null;
					foreach (var a in re.Args)
					{
						try
						{
							await al.Add(a, async () =>
							{
								var p = MsgProviderResolver(a.MsgProviderId);
								if (string.IsNullOrEmpty(a.Target))
								{
									if (!a.TargetId.HasValue || string.IsNullOrEmpty(a.Target = await p.TargetResolve(a.TargetId.Value)))
										throw new ArgumentException($"消息未指定发送目标：Provider:{a.MsgProviderId} {message.ToString()}");
								}
								return await p.Send(a);
							});
						}
						catch (Exception ex)
						{
							if (Errors == null)
								Errors = new List<Exception>();
							Errors.Add(ex);
						}
					}
					if (Errors != null)
					{
						if (Errors.Count == 1)
							throw Errors[0];
						else
							throw new AggregateException(Errors.ToArray());
					}
					return re.PolicyId;
				}
			);

		}
	}
}

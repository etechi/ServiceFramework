using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.TextMessages
{

	public class DefaultMsgLogger : IMsgLogger
	{
		public ILogger Logger { get; }
		public DefaultMsgLogger(ILogger<DefaultMsgLogger> Logger)
		{
			this.Logger = Logger;
        }

        public virtual Task<object> PreSend(IServiceInstanceDescriptor Service, Message message, long? targetUserId,string[] targets)
        {
            var text = $"消息服务:{Service.ServiceImplement.Name}/{Service.ServiceDeclaration.ServiceName} {message} 用户:{targetUserId} 目标:{targets.Join(";")}";
            Logger.Info($"开始发送消息: {text}");
            return Task.FromResult<object>(text);
        }
        public virtual Task PostSend(object Context,IEnumerable<MessageSendResult> results, Exception error)
		{
            var text = (string)Context;
            var re = results == null ? "消息服务已被禁止" : results.Select(r => 
                r.Target + "=" + (r.Exception == null ? r.Result : r.Exception.Message)
                ).Join("&");

            Logger.Info(error,$"发送消息完成: {text} 结果:{re}");
			return Task.CompletedTask;
		}
	}

}

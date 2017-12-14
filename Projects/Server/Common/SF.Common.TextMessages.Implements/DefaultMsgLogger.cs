using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.TextMessages
{

	public class DefaultMsgLogger : IMsgLogger,IMsgActionLogger
	{
		public ILogger Logger { get; }
		public DefaultMsgLogger(ILogService LogService)
		{
			this.Logger = LogService.GetLogger("文本消息");
        }


		public virtual async Task<long> Add(long? targetUserId, Message message, Func<IMsgActionLogger, Task<long>> Action)
		{
			var text = $"用户:{targetUserId} 消息:{message.ToString()}";

			Logger.Info($"开始处理: {text}");
			try
			{
				var re= await Action(this);
				Logger.Info($"处理完成: {text} ");
				return re;
			}
			catch(Exception error)
			{
				Logger.Info(error, $"处理异常: {text} 异常:{error}");
				throw;
			}
		}

		async Task<string> IMsgActionLogger.Add(MsgSendArgument Arg, Func<Task<string>> Action)
		{
			var text =Arg.ToString();

			Logger.Info($"开始发送: {text}");
			try
			{
				var re=await Action();
				Logger.Info($"发送完成: {text} 结果:{re}");
				return re;
			}
			catch (Exception error)
			{
				Logger.Info(error, $"发送异常: {text} 异常:{error}");
				throw;
			}
		}
	}

}

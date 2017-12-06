using System.Threading.Tasks;

namespace SF.Common.TextMessages
{
	public enum SendStatus
    {
		/// <summary>
		/// 发送中
		/// </summary>
		Sending,
		/// <summary>
		/// 发送成功
		/// </summary>
		Completed,
		/// <summary>
		/// 发送失败
		/// </summary>
		Failed
	}
	public interface IMsgService
	{
        Task<string> Send(
            string SysServiceType,
            long? targetUserId,
            string address,
            Message message
            );
        Task<string> Send(
		   long SysServiceId,
		   long? targetUserId,
           string address,
           Message message
           );
        Task<string[]> Send(
            string SysServiceType,
			long? TargetUserId,
            string[] addresses, 
            Message message
            );
		Task<string[]> Send(
			long SysServiceId, 
            string[] addresses, 
            Message message
            );
	}

}

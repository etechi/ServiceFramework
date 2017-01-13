using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.TextMessages
{
    public enum SendStatus
    {
        [Display(Name ="发送中")]
        Sending,
        [Display(Name = "发送成功")]
        Completed,
        [Display(Name = "发送失败")]
        Failed
    }
	public interface IMsgService
	{
        Task<string> Send(
            string SysServiceType,
            int? targetUserId,
            string address,
            Message message
            );
        Task<string> Send(
           int SysServiceId,
           int? targetUserId,
           string address,
           Message message
           );
        Task<string[]> Send(
            string SysServiceType,
            int? TargetUserId,
            string[] addresses, 
            Message message
            );
		Task<string[]> Send(
            int SysServiceId, 
            string[] addresses, 
            Message message
            );
	}

}

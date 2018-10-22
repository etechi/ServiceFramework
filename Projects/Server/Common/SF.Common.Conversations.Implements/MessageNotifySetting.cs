
using System.Threading.Tasks;
using System.Data.Common;
using System;
using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using SF.Sys.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using SF.Sys.Services.Management.Models;
using System.Collections.Generic;

namespace SF.Common.Conversations
{

    /// <summary>
    /// 消息通知设置
    /// </summary>
    public class MessageNotifySetting
    {
        /// <summary>
        /// 最小通知延时(秒)
        /// </summary>
        public int MinMessageNotifyDelaySeconds { get; set; }

        /// <summary>
        /// 最大通知延时(秒)
        /// </summary>
        public int MaxMessageNotifyDelaySeconds { get; set; }


        /// <summary>
        /// 最小通知间隔(分钟)
        /// </summary>
        public int MinNotifyIntervalMinutes { get; set; }

        /// <summary>
        /// 消息过期时间(小时)
        /// </summary>
        public int MaxMessageExpireHours { get; set; }
    }
	
}
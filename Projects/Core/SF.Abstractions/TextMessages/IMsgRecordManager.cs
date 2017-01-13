using SF.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.TextMessages
{
    [EntityObject("文本消息记录")]
    public class MsgRecord
    {
        [Key]
        [TableVisible]
        [Display(Name ="Id")]
        public long Id { get; set; }

        [Display(Name = "状态")]
        [TableVisible]
        public SendStatus Status { get; set; }

        [Display(Name = "目标用户")]
        [EntityIdent("用户", nameof(TargetUserName))]
        public int? TargetUserId { get; set; }

        [Display(Name = "目标用户")]
        [Ignore]
        [TableVisible]
        public string TargetUserName { get; set; }

        [Display(Name = "接收方")]
        [TableVisible]
        public string Targets { get; set; }

        [Display(Name = "摘要")]
        [Ignore]
        [TableVisible]
        public string Summary { get; set; }

        [EntityIdent("系统服务", nameof(ServiceName))]
        [Display(Name = "发送服务")]
        public int ServiceId { get; set; }

        [Ignore]
        [Display(Name = "发送服务")]
        [TableVisible]
        public string ServiceName { get; set; }

        [Display(Name = "发送方")]
        public string Sender { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "内容")]
        [MultipleLines]
        public string Body { get; set; }

        [Display(Name = "消息头部")]
        [MultipleLines]
        public string Headers { get; set; }

        [Display(Name = "消息参数")]
        [MultipleLines]
        public string Args { get; set; }

        [Display(Name = "发送时间")]
        [TableVisible]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "完成时间")]
        public DateTime? CompletedTime { get; set; }

        [Display(Name = "错误信息")]
        [MultipleLines]
        public string Error { get; set; }

        [Display(Name ="单项错误信息")]
        [MultipleLines]
        public string TargetResults { get; set; }

        [Display(Name = "跟踪对象")]
        [EntityIdent]
        public string TrackEntityId { get; set; }

        
    }

    public class MsgRecordQueryArgument
    {
        [Display(Name ="状态")]
        public SendStatus? Status { get; set; }

        [Display(Name = "目标用户")]
        [EntityIdent("用户")]
        public int? TargeUserId { get; set; }


        [Display(Name = "发送服务")]
        [EntityIdent("系统服务")]
        public int? ServiceId { get; set; }

        [Display(Name = "发送时间")]
        public QueryRange<DateTime> Time { get; set; }

        [Display(Name ="发送对象")]
        public string Target { get; set; }
    }
    public interface IMsgRecordManager
	{
        Task<QueryResult<MsgRecord>> Query(MsgRecordQueryArgument Arg, Paging Paging);
        Task<MsgRecord> Get(int Id);
    }

}

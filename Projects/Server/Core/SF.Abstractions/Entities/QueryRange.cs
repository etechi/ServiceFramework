using System;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Entities
{

	public class QueryRange<T>
		where T :struct,IComparable<T>
	{
		[Comment(Prompt ="开始")]
		public virtual T? Begin { get; set; }
		[Comment(Prompt = "结束")]
		public virtual T? End { get; set; }
	}
    public class DateQueryRange:
       QueryRange<DateTime>
    {
        [Comment(Prompt = "开始(默认为最近31天)")]
        public override DateTime? Begin { get; set; }
        [Date(EndTime=true)]
        public override DateTime? End { get; set; }
    }

    public class NullableQueryRange<T>
        where T : struct, IComparable<T>
    {
        public virtual bool NotNull { get; set; }
        [Comment(Prompt = "开始")]
        public virtual T? Begin { get; set; }
        [Comment(Prompt = "结束")]
        public virtual T? End { get; set; }
    }
    public class NullableDateQueryRange :
       NullableQueryRange<DateTime>
    {
        [Date(EndTime = true)]
        public override DateTime? End { get; set; }
    }
    public enum QueryableBoolean
    {
        [Comment(Name = "是")]
        True,
        [Comment(Name = "否")]
        False
    }
    
}

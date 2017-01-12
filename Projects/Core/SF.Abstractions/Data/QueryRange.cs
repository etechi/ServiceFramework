using SF.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SF.Reflection;
namespace SF.Data
{

	public class QueryRange<T>
		where T :struct,IComparable<T>
	{
		[Display(Prompt ="开始")]
		public virtual T? Begin { get; set; }
		[Display(Prompt = "结束")]
		public virtual T? End { get; set; }
	}
    public class DateQueryRange:
       QueryRange<DateTime>
    {
        [Display(Prompt = "开始(默认为最近31天)")]
        public override DateTime? Begin { get; set; }
        [Date(EndTime=true)]
        public override DateTime? End { get; set; }
    }

    public class NullableQueryRange<T>
        where T : struct, IComparable<T>
    {
        public virtual bool NotNull { get; set; }
        [Display(Prompt = "开始")]
        public virtual T? Begin { get; set; }
        [Display(Prompt = "结束")]
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
        [Display(Name = "是")]
        True,
        [Display(Name = "否")]
        False
    }
    
}

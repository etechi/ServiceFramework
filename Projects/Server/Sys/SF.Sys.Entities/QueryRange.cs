#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.Annotations;
using SF.Sys.Annotations;

namespace SF.Sys.Entities
{

	public class QueryRange<T>
		where T :struct,IComparable<T>
	{
		/// <prompt>
		/// 开始
		/// </prompt>
		public virtual T? Begin { get; set; }
		/// <prompt>
		/// 结束
		/// </prompt>
		public virtual T? End { get; set; }
	}
    public class DateQueryRange:
       QueryRange<DateTime>
    {
		/// <prompt>
		/// 开始(默认为最近31天)
		/// </prompt>
        public override DateTime? Begin { get; set; }
        [Date(EndTime=true)]
        public override DateTime? End { get; set; }
    }

    public class NullableQueryRange<T>
        where T : struct, IComparable<T>
    {
        public virtual bool NotNull { get; set; }
		/// <prompt>
		/// 开始
		/// </prompt>
        public virtual T? Begin { get; set; }
		/// <prompt>
		/// 结束
		/// </prompt>
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
		/// <summary>
		/// 是
		/// </summary>
		True,
		/// <summary>
		/// 否
		/// </summary>
		False
	}
    
}

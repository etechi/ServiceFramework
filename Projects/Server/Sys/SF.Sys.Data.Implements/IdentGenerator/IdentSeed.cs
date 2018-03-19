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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Sys.Data.IdentGenerator.DataModels
{
	[Table("IdentSeed")]
	public class IdentSeed
	{
		/// <summary>
		/// 类型
		/// </summary>
		[Key]
		[MaxLength(200)]
		public string Type { get; set; }


		/// <summary>
		/// 下一个标识值
		/// </summary>
		public long NextValue { get; set; }

		///<title>标识值分段</title>
		/// <summary>
		/// 标识分段变化时，将重新开始生成标识值
		/// </summary>
		public int Section { get; set; }

		/// <summary>
		/// 乐观锁时间戳
		/// </summary>
		[ConcurrencyCheck]
		[Timestamp]
		public byte[] TimeStamp { get; set; }
	}
}

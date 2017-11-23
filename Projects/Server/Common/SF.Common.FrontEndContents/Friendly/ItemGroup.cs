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

using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.Entities.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.FrontEndContents.Friendly
{
	public class ItemGroup<T> : 
		IEntityWithId<long> 
		where T:LinkItemBase 
	{
        [Key]
        [Ignore]
        public long Id { get; set; } = 1;

		/// <summary>
		/// 名称
		/// </summary>
		[TableVisible]
		[ReadOnly(true)]
		[Ignore]
		public string Name { get; set; }

		/// <summary>
		/// 帮助
		/// </summary>
		[TableVisible]
		[ReadOnly(true)]
		[Ignore]
		public string Help { get; set; }

		[TableRows]
        public T[] Items { get; set; }
	}
}

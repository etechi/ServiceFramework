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

using SF.Data;
using SF.Entities.DataModels;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Management.SysAdmins.Entity.DataModels
{
	[Table("MgrSysAdmin")]
	public class SysAdmin<TSysAdmin> : ObjectEntityBase
		where TSysAdmin: SysAdmin<TSysAdmin>
	{

		[Comment("账号")]
		[MaxLength(100)]
		[Index]
		public string Account { get; set; }

		[Comment("图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

	}
	public class SysAdmin : SysAdmin<SysAdmin>
	{ }

}


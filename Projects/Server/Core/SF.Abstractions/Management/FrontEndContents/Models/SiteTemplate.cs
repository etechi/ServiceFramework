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

using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents
{
	[EntityObject]
    public class SiteTemplate:IEntityWithId<long>
    {
		[Key]
		[Display(Name = "ID", Prompt = "保存后自动产生")]
		[ReadOnly(true)]
		[TableVisible]
		public long Id { get; set; }

		[TableVisible]
		[Display(Name = "名称")]
		[StringLength(100)]
		[Required]
		public string Name { get; set; }

		//[Display(Name = "模板")]
		[Required]
		public SiteConfigModels.SiteModel Model { get; set; }

    }
}

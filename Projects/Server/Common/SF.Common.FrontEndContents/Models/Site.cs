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

using SF.Sys.Entities;
using SF.Sys.Entities.Annotations;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.FrontEndContents
{
	[EntityObject]
    public class Site : IEntityWithId<string>
	{
		/// <summary>
		/// Id
		/// </summary>
		/// <prompt>保存后自动产生</prompt>
		[Key]
		[TableVisible]
		[StringLength(20)]
		public string Id { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[TableVisible]
		[StringLength(100)]
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 站点模板
		/// </summary>
		[EntityIdent(typeof(SiteTemplate), nameof(TemplateName))]
		[Required]
		public long TemplateId { get; set; }

		/// <summary>
		/// 站点模板
		/// </summary>
		[TableVisible]
		[Ignore]
		public string TemplateName { get; set; }

       
    }
}

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

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;
using SF.Sys.Entities;

namespace SF.Common.FrontEndContents.DataModels
{

	/// <summary>
	/// 站点配置
	/// </summary>
	/// <typeparam name="TSite"></typeparam>
	/// <typeparam name="TSiteTemplate"></typeparam>
	[Table("FrontSite")]
    public class Site<TSite, TSiteTemplate> :
		IEntityWithId<string>
		where TSite : Site<TSite, TSiteTemplate>
		where TSiteTemplate : SiteTemplate<TSite, TSiteTemplate>
	{
		/// <summary>
		/// Id
		/// </summary>
		[Key]
		[MaxLength(100)]
		public string Id{get;set;}

		/// <summary>
		/// 站点名称
		/// </summary>
		[MaxLength(100)]
        public string Name { get; set; }

		/// <summary>
		/// 站点模板ID
		/// </summary>
		[Index]
        public long TemplateId{get;set;}

		[ForeignKey(nameof(TemplateId))]
		public TSiteTemplate Template { get; set; }
	}

	public class Site : Site<Site, SiteTemplate>
	{ }
}

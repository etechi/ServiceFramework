﻿#region Apache License Version 2.0
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
using SF.Sys.Entities.Annotations;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.FrontEndContents.Friendly
{
	public class ImageTextItem : LinkItemBase
    {

		/// <summary>
		/// 图片
		/// </summary>
		[Image]
        [Layout(1)]
        public string Image { get; set; }

		/// <summary>
		/// 文字1
		/// </summary>
		[Required]
        [Layout(2)]
        public string Text1 { get; set; }

		/// <summary>
		/// 文字2
		/// </summary>
		[Layout(3)]
        public string Text2 { get; set; }
    }
}

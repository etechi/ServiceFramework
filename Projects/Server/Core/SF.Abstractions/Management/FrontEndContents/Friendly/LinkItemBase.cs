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

using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
{

	public enum LinkTarget
    {
        [Display(Name = "默认")]
        _default,
        [Display(Name = "新窗口")]
        _blank,
        [Display(Name = "当前页")]
        _self
    }
    public class LinkItemBase
	{
        [Display(Name ="链接")]
        [Layout(100)]
        public string Link { get; set; }

        [Display(Name = "打开位置")]
        [Layout(101)]
        public LinkTarget LinkTarget { get; set; } = LinkTarget._default;
	}
    
}

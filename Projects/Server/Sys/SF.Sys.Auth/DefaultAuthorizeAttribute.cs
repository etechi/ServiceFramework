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
using System.Collections.Generic;
using System.Text;

namespace SF.Sys.Auth
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false)]
	public class AnonymousAllowedAttribute : Attribute
	{

	}
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface,AllowMultiple = true)]
	public class DefaultAuthorizeAttribute : Attribute
	{
		public string RoleIdent { get; set; }
		public string RoleName { get; set; }
		public bool ReadOnly { get; set; }
		public DefaultAuthorizeAttribute():this(null,null)
		{

		}
		public DefaultAuthorizeAttribute(string RoleIdent, bool ReadOnly):this(RoleIdent,null,ReadOnly)
		{

		}
		public DefaultAuthorizeAttribute(string RoleIdent,string RoleName=null,bool ReadOnly=false)
		{
			this.RoleIdent = RoleIdent;
			this.RoleName = RoleName;
			this.ReadOnly = ReadOnly;
		}
	}

	
}

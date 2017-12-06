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

using SF.Sys.Metadata.Models;

namespace SF.Sys.NetworkService.Metadata
{
	public class GrantInfo
	{
		public bool UserRequired { get; set; }
		public string[] RolesRequired { get; set; }
		public string[] PermissionsRequired { get; set; }
	}
	public class Service : SF.Sys.Metadata.Models.Entity
	{
        private System.Type ServiceSysType { get; }
        public System.Type GetSysType() => ServiceSysType;

        public Service(System.Type SysType)
        {
            this.ServiceSysType = SysType;
        }

        public Method[] Methods { get; set; }

		public GrantInfo GrantInfo { get; set; }
	}

	public class Method : SF.Sys.Metadata.Models.Method
	{
        private System.Reflection.MethodInfo ServiceSysMethod { get; }
        public System.Reflection.MethodInfo GetSysMethod() => ServiceSysMethod;

		public Method(System.Reflection.MethodInfo Method)
        {
            this.ServiceSysMethod = Method;
        }

        public string HeavyParameter { get; set; }

		public GrantInfo GrantInfo { get; set; }
	}

	public class Parameter : SF.Sys.Metadata.Models.Parameter
	{
		//public bool TransportMode { get; set; }
	}
	public class Library: SF.Sys.Metadata.Models.Library
	{
		public Service[] Services { get; set; }
	}
}

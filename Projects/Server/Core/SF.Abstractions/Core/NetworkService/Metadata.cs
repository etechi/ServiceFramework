using SF.Metadata.Models;

namespace SF.Core.NetworkService.Metadata
{
	public class GrantInfo
	{
		public bool UserRequired { get; set; }
		public string[] RolesRequired { get; set; }
		public string[] PermissionsRequired { get; set; }
	}
	public class Service : SF.Metadata.Models.Entity
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

	public class Method : SF.Metadata.Models.Method
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

	public class Parameter : SF.Metadata.Models.Parameter
	{
		//public bool TransportMode { get; set; }
	}
	public class Library: SF.Metadata.Models.Library
	{
		public Service[] Services { get; set; }
	}
}

using SF.Metadata.Models;

namespace SF.Services.Metadata.Models
{
	public class GrantInfo
	{
		public bool UserRequired { get; set; }
		public string[] RolesRequired { get; set; }
		public string[] PermissionsRequired { get; set; }
	}
	public class Service : Entity
	{
        private System.Type _SysType { get; }
        public System.Type GetSysType() => _SysType;

        public Service(System.Type SysType)
        {
            this._SysType = SysType;
        }

        public Method[] Actions { get; set; }

		public GrantInfo GrantInfo { get; set; }
	}

	public class Method : SF.Metadata.Models.Method
	{
        private System.Reflection.MethodInfo _SysMethod { get; }
        public System.Reflection.MethodInfo GetSysMethod() => _SysMethod;

		public Method(System.Reflection.MethodInfo Method)
        {
            this._SysMethod = Method;
        }

        public string NetworkMethod { get; set; }

		public GrantInfo GrantInfo { get; set; }
	}

	public class Parameter : SF.Metadata.Models.Parameter
	{
		public string TransportMode { get; set; }
	}
	public class Library: SF.Metadata.Models.Library
	{
		public Service[] Services { get; set; }
	}
}

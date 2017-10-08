using System;

namespace SF.Core.ServiceManagement
{
	[AttributeUsage(AttributeTargets.Interface| AttributeTargets.Class)]
	public class AutoBindAttribute: Attribute
	{

	}

	[AutoBind]
	public interface IServiceInstanceDescriptor
	{
		long InstanceId { get; }
		long? ParentInstanceId { get; }
		bool IsManaged { get; }
		IServiceDeclaration ServiceDeclaration { get; }
		IServiceImplement ServiceImplement { get; }
		IDisposable OnSettingChanged<T>(Action<T> Callback);
	}

}

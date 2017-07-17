using System;


namespace SF.Core.ServiceManagement
{
	[UnmanagedService]
	public interface IServiceDetector
	{
		bool IsService(Type type);
	}
	static class ServiceDetectorExtension
	{
	}
}

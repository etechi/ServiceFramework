using System;


namespace SF.Core.ServiceManagement
{
	public interface IServiceDetector
	{
		bool IsService(Type type);
	}
	static class ServiceDetectorExtension
	{
	}
}

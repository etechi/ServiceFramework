using System;


namespace SF.Services.ManagedServices.Runtime
{
	public enum ServiceType
	{
		Unknown,
		Normal,
		Managed
	}
	public interface IServiceDetector
	{
		ServiceType GetServiceType(Type type);
	}
	static class ServiceDetectorExtension
	{
		public static bool IsServiceType(this IServiceDetector sd,Type type)
		{
			return sd.GetServiceType(type) != ServiceType.Unknown;
		}
	}
}

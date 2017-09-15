using System;


namespace SF.Services.Management.Internal
{
	enum ServiceType
	{
		Unknown,
		Normal,
		Managed
	}
	interface IServiceDetector
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

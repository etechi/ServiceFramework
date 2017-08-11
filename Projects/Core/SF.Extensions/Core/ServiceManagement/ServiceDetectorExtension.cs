using System;
using System.Collections.Generic;
using System.Reflection;

namespace SF.Core.ServiceManagement
{


	public enum ServiceResolveType
	{
		None,
		Direct,
		Lazy,
		Func,
		ResolveByIdent,
		Enumerable
	}
	public static class ServiceDetectorExtension
	{
		public static ServiceResolveType DetectService(this IServiceDetector Detector, Type Type, out Type RealType)
		{
			ServiceResolveType ResolveType = ServiceResolveType.Direct;
			RealType = Type;

			if (Type.IsGeneric())
			{
				var gtd = Type.GetGenericTypeDefinition();
				if (gtd == typeof(Lazy<>))
					ResolveType = ServiceResolveType.Lazy;
				else if (gtd == typeof(Func<>))
					ResolveType = ServiceResolveType.Func;
				else if (gtd == typeof(TypedInstanceResolver<>))
					ResolveType = ServiceResolveType.ResolveByIdent;
				else if (gtd == typeof(IEnumerable<>))
					ResolveType = ServiceResolveType.Enumerable;

				if (ResolveType != ServiceResolveType.Direct)
					RealType = Type.GetGenericArguments()[0];
			}
			if (Detector.IsService(RealType))
				return ResolveType;
			if (RealType.IsGeneric() && Detector.IsService(RealType.GetGenericTypeDefinition()))
				return ResolveType;
			if (ResolveType != ServiceResolveType.Direct)
				throw new InvalidOperationException($"未定义服务:{RealType}");
			return ServiceResolveType.None;
		}
		public static ServiceResolveType DetectService(this IServiceDetector Detector, Type Type)
		{
			return Detector.DetectService(Type, out var rt);
		}
	}
}

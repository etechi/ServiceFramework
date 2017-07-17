using System;

using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public interface IServiceImplement
	{
		string ImplementName { get; }
		Type ServiceType { get; }
		Type ImplementType { get; }
		object ImplementInstance { get; }
		Func<IServiceProvider, object> ImplementCreator { get; }

		ServiceImplementType ServiceImplementType { get; }
		ServiceImplementLifetime LifeTime { get; }
		bool IsManagedService { get; }
	}
	public interface IServiceDeclaration
	{
		string ServiceName { get; }
		Type ServiceType { get; }
		IReadOnlyList<IServiceImplement> Implements { get; }
	}
	[UnmanagedService]
	public interface IServiceMetadata : IServiceDetector
	{
		IReadOnlyDictionary<string, IServiceDeclaration> ServicesByTypeName { get; }
		IReadOnlyDictionary<string, IServiceImplement[]> ImplementsByTypeName { get; }
		IReadOnlyDictionary<Type, IServiceDeclaration> Services { get; }
	}

}

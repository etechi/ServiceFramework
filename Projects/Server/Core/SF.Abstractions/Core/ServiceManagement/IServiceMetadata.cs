using System;

using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public interface IServiceImplement
	{
		string Name { get; }
		string ImplementName { get; }
		Type ServiceType { get; }
		Type ImplementType { get; }
		object ImplementInstance { get; }
		Func<IServiceProvider, object> ImplementCreator { get; }
		System.Reflection.MethodInfo ImplementMethod { get; }
		ServiceImplementType ServiceImplementType { get; }
		ServiceImplementLifetime LifeTime { get; }
		bool IsManagedService { get; }
		IManagedServiceInitializer ManagedServiceInitializer { get; }
	}

	public interface IServiceDeclaration
	{
		bool HasManagedServiceImplement{get;}
		bool HasUnmanagedServiceImplement { get; }
		string ServiceName { get; }
		Type ServiceType { get; }
		IReadOnlyList<IServiceImplement> Implements { get; }
	}
	public interface IServiceMetadata : IServiceDetector
	{
		IReadOnlyDictionary<string, IServiceDeclaration> ServicesById { get; }
		IReadOnlyDictionary<string, IServiceImplement> ImplementsById { get; }
		IReadOnlyDictionary<string, IServiceDeclaration> ServicesByTypeName { get; }
		IReadOnlyDictionary<string, IServiceImplement[]> ImplementsByTypeName { get; }
		IReadOnlyDictionary<Type, IServiceDeclaration> Services { get; }
	}

}

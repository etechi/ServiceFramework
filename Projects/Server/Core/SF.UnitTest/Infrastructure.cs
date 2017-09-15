using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data.Storage;

namespace SF.UnitTest.Infrastructures
{
	public interface ISystem
	{

	}
	public interface IProcess
	{

	}
	public interface IDomain
	{

	}
	public interface IService
	{

	}

	public interface IDataEntity
	{

	}

	public interface IApplication
	{

	}

	public interface IServiceInstance
	{

	}
	public interface IServiceInterface
	{

	}
	public interface IServiceBiznessInterface
	{

	}

	public interface IServiceInterfaceResolver
	{
		object Resolve(Type ServiceType, long InstanceId, Type ServiceInterface);
	}

	public interface IServiceUserManagementInterface
	{

	}
	public interface IServiceSystemManagementInterface
	{

	}
	public interface IServiceInterfaceImplement
	{

	}


	public interface IServiceCategory
	{

	}

}

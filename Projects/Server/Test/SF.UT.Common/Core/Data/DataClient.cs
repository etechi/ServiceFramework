using System;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using SF.Data.Entity;
using SF.Data.Storage;

namespace SF.UT.Data
{
	public class QueryResult<T> {
		T[] Items { get; set; }
	}
	public struct Option<T>
	{
		public T Value { get; set; }
		public bool HasValue { get; set; }
	}
	public interface IQueryArgument<TKey>
	{
		Option<TKey> Id { get; }
	}
	public interface IEntityQueryable<TEntity, TQueryArgument>
	{
		Task<QueryResult<TEntity>> Query(TQueryArgument Arg);
	}
	public interface IEntityLoader<TKey, TEntity>
	{
		Task<TEntity> Load(TKey Id);
	}
	public interface IEntityBatchLoader<TKey, TEntity>
	{
		Task<TEntity[]> Load(TKey[] Ids);
	}
	public interface IEntitySource<TKey, TEntity> :
	   IEntityLoader<TKey, TEntity>,
	   IEntityBatchLoader<TKey, TEntity>,
	   IEntityQueryable<TKey, UserQueryArgument>
	{

	}
	public interface IEntityManager<TKey, TEntity>
	{
		Task<TEntity> LoadForUpdate(TKey Id);
		Task<TKey> Create(TEntity Entity);
		Task Update(TEntity Entity);
		Task Delete(TKey Key);
	}
	public class UserQueryArgument : IQueryArgument<string>
	{
		public Option<string> Id { get; set; }
		public string FirstName { get; set; }
	}


	public class EntitySource<TKey, TEntity> :
		IEntitySource<TKey, TEntity>
	{
		protected IDataContext Context { get; }
		public EntitySource(IDataContext Context)
		{
			this.Context = Context;
		}
		public Task<TEntity[]> Load(TKey[] Ids)
		{
			throw new NotImplementedException();
		}

		public Task<TEntity> Load(TKey Id)
		{
			throw new NotImplementedException();
		}

		Task<QueryResult<TKey>> IEntityQueryable<TKey, UserQueryArgument>.Query(UserQueryArgument Arg)
		{
			throw new NotImplementedException();
		}
	}
	public class EntityManager<TKey, TEntity, TEntityEditable> : EntitySource<TKey, TEntity>, IEntityManager<TKey, TEntityEditable>
	{
		public EntityManager(IDataContext Context) : base(Context) { }

		public Task<TKey> Create(TEntityEditable Entity)
		{
			throw new NotImplementedException();
		}

		public Task Delete(TKey Key)
		{
			throw new NotImplementedException();
		}

		public Task<TEntityEditable> LoadForUpdate(TKey Id)
		{
			throw new NotImplementedException();
		}

		public Task Update(TEntityEditable Entity)
		{
			throw new NotImplementedException();
		}
	}

	public interface IUserManager :
		IEntityManager<string, UserEditable>,
		IEntitySource<string, User>
	{

	}

	public interface UserModels
	{
		IDataSet<DataModels.User> Users { get; }
		IDataSet<DataModels.Post> Posts { get; }
	}
	public class UserManager :
		EntityManager<string, User, UserEditable>,
		IUserManager
	{
		public UserManager(IDataContext Context) : base(Context) { }
	}

	public static class ServiceCollectionExtension
	{
		public static void UseUserManager(this SF.Core.DI.IDIServiceCollection sc)
		{

		}
	}

	public class Client
    {
		[Fact]
		public void Test()
		{
		
		}
	}
}

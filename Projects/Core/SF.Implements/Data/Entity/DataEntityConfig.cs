using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.DI;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using SF.Metadata;

namespace SF.Data.Entity
{
	abstract class DataEntityConfigItem
	{
		public string Name { get; }
		public Type EntityType { get; }
		Lazy<Func<object, object[]>> GetIdentFunc{get;}
		Lazy<Func<object, string>> GetNameFunc { get; }

		public DataEntityConfigItem(string Name,Type EntityType)
		{
			this.Name = Name;
			this.EntityType = EntityType;
			GetIdentFunc = new Lazy<Func<object, object[]>>(BuildGetIdentFunc);
			GetNameFunc = new Lazy<Func<object, string>>(BuildGetNameFunc);
		}
		protected abstract Func<object, string> BuildGetNameFunc();
		protected abstract Func<object, object[]> BuildGetIdentFunc();
		public abstract Task<IDataEntity[]> Load(IServiceProvider sp, string[] Idents);
		public string GetIdent(object Instance) => Instance==null?null:Name+"-"+String.Join("-", GetIdentFunc.Value(Instance));
		public string GetName(object Instance) => Instance == null ? null : GetNameFunc.Value?.Invoke(Instance);
	}
	class DataEntityConfigItem<TKey,TEntity> : DataEntityConfigItem
	{
		public DataEntityConfigItem(string Name) : base(Name, typeof(TEntity))
		{
		}

		public override async Task<IDataEntity[]> Load(IServiceProvider sp, string[] Idents)
		{
			if (Idents == null || Idents.Length == 0)
				return Array.Empty<IDataEntity>();
			if (Idents.Length == 1)
			{
				var el = sp.Resolve<IEntityLoader<TKey, TEntity>>();
				var ins = await el.GetAsync((TKey)Convert.ChangeType(Idents[0], typeof(TKey)));
				return new[] { new DataEntity(ins, this) };
			}
			else
			{
				var mel = sp.TryResolve<IEntityBatchLoader<TKey, TEntity>>();
				if (mel != null)
					return (await mel.GetAsync(
						Idents.Select(id => (TKey)Convert.ChangeType(id, typeof(TKey))).ToArray()
						))
						.Select(ins => new DataEntity(ins, this))
						.ToArray();

				var re = new List<IDataEntity>();
				var el = sp.Resolve<IEntityLoader<TKey, TEntity>>();
				foreach (var id in Idents.Select(id => (TKey)Convert.ChangeType(id, typeof(TKey))))
				{
					var ins = await el.GetAsync(id);
					if (ins != null)
						re.Add(new DataEntity(ins, this));
				}
				return re.ToArray();
			}
		}


		protected override Func<object, object[]> BuildGetIdentFunc()
		{
			var arg = Expression.Parameter(typeof(object), "o");
			return Expression.Lambda<Func<object, object[]>>(
				Expression.NewArrayInit(
					typeof(object),
					EntityType
						.GetProperties(BindingFlags.Public | BindingFlags.Instance)
						.Where(p => p.GetCustomAttribute<KeyAttribute>(true) != null)
						.OrderBy(p => p.GetCustomAttribute<ColumnAttribute>(true)?.Order ?? 0)
						.Select(p =>
						{
							var e = (Expression)Expression.Property(Expression.Convert(arg, typeof(TEntity)), p);
							if (p.PropertyType.IsValue())
								e = Expression.Convert(e, typeof(object));
							return e;
						})
						.ToArray()
						),
					arg
				).Compile();
		}

		protected override Func<object, string> BuildGetNameFunc()
		{
			var arg = Expression.Parameter(typeof(object), "o");
			var props = EntityType
						.GetProperties(BindingFlags.Public | BindingFlags.Instance )
						.ToArray();

			var nameProp = props.FirstOrDefault(p => p.GetCustomAttribute<EntityTitleAttribute>(true)!=null);
			if (nameProp == null)
				nameProp = props.FirstOrDefault(p => p.Name == "Name" || p.Name == "Title" || p.Name == "name" || p.Name == "title");
			if (nameProp == null)
				return null;
			if (nameProp.PropertyType != typeof(string))
				throw new InvalidOperationException();

			return Expression.Lambda<Func<object, string>>(
					Expression.Property(Expression.Convert(arg, typeof(TEntity)), nameProp),
					arg
					).Compile();
		}
	}
	class DataEntityConfigCache 
	{
		Dictionary<string, DataEntityConfigItem> Items { get; }
		public DataEntityConfigItem FindConfigItem(string Name)
		{
			return Items.Get(Name);
		}
		public DataEntityConfigCache(IEnumerable<DataEntityConfigItem> Items)
		{
			this.Items = Items.ToDictionary(p => p.Name);
		}
	}
}

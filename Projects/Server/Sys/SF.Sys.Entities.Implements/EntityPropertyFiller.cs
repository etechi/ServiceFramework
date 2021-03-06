﻿#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Sys.Annotations;
using SF.Sys.Linq;
using SF.Sys.Linq.Expressions;
using SF.Sys.Reflection;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Sys.Entities
{
	public class EntityPropertyFiller : IEntityPropertyFiller
	{
		abstract class EntityFiller<TItem>
		{
			public abstract Task Fill(IServiceProvider sp, long? ScopeId, TItem[] Items);
		}
		class EntityFiller<TKey,TEntity, TManager, TItem> :
			EntityFiller<TItem>
			where TManager : class, IEntityLoadable<ObjectKey<TKey>, TEntity>
			where TKey : IEquatable<TKey>
			where TEntity:class,IEntityWithId<TKey>
		{
			Action<TItem, List<TKey>> CollectIdents { get; }
			Action<TItem, TEntity>[] FillFields { get; }
			bool FillRequired { get; }
			string[] Properties { get;  }
			public EntityFiller(
				(PropertyInfo IdProp, Type EntityType,Type ManagerType, (PropertyInfo dstProp, PropertyInfo srcProp)[] ValueProps)[] settings
				)
			{
				var varName = Expression.Variable(typeof(string));
				var ArgItem = Expression.Parameter(typeof(TItem));
				var ArgNames = Expression.Parameter(typeof(IReadOnlyList<string>));


				FillRequired = settings.Length > 0;
				if (!FillRequired)
					return;

				Properties = settings
					.SelectMany(p => p.ValueProps)
					.Select(p => p.srcProp.Name)
					.Union(
						settings[0].EntityType
						.AllPublicInstanceProperties()
						.Where(p=>p.GetCustomAttribute<KeyAttribute>()!=null)
						.Select(p=>p.Name)
					)
					.Distinct()
					.ToArray();

				var argItem = Expression.Parameter(typeof(TItem));
				var argList = Expression.Parameter(typeof(List<TKey>));

				
				var varKey = typeof(TKey).IsClass?null: Expression.Variable(typeof(Nullable<>).MakeGenericType(typeof(TKey)));
				var list = new List<Expression>();
				foreach (var s in settings)
				{
					var isNullable = varKey!=null && s.IdProp.PropertyType.GetGenericArgumentTypeAsNullable() != null;
					if (isNullable)
					{
						list.Add(varKey.Assign(argItem.GetMember(s.IdProp)));
						list.Add(
							argList.CallMethod(
								"Add",
								Expression.Condition(
								varKey.GetMember("HasValue"),
								varKey.GetMember("Value"),
								Expression.Constant(default(TKey)) //没有数据时填空，否则无法对应
								)
							)
						);
					}
					else
					{
						list.Add(
							argList.CallMethod(
								"Add",
								argItem.GetMember(s.IdProp)
								)
						);
					}
				}
				CollectIdents = 
					
					Expression.Lambda<Action<TItem, List<TKey>>>(
						(varKey != null ?
							Expression.Block(
								new[] { varKey },
								list
								):
							Expression.Block(
									list
								)),
						argItem,
						argList
					).Compile();
				

				var argEntity = Expression.Parameter(typeof(TEntity));
				FillFields = (from setting in settings
							  select BlockExpression.Block(
									  setting.ValueProps.Select(ep =>
									  {
										  Expression exp = argEntity.GetMember(ep.srcProp);
										  if (!ep.dstProp.PropertyType.IsAssignableFrom(ep.srcProp.PropertyType))
										  {
											  var convert = typeof(Convert).GetMethod(
														"ToString",
														BindingFlags.Public | BindingFlags.Static,
														null,
														new[] { ep.srcProp.PropertyType },
														null
														);
											  if (convert != null)
												  exp = Expression.Call(convert, exp);
											  else
												  exp = Expression.Call(
													  typeof(Convert).GetMethod(
														"ToString",
														BindingFlags.Public | BindingFlags.Static,
														null,
														new[] { typeof(object) },
														null
														),
													  ep.srcProp.PropertyType.IsValueType ?
													  Expression.Convert(exp, typeof(object)) :
													  exp
													  );
										  }
										  return argItem.SetProperty(ep.dstProp, exp);
									  }
									  )
								  ).Compile<Action<TItem, TEntity>>(
								  argItem,
								  argEntity
								  )
							).ToArray();
			}
			public override async Task Fill(IServiceProvider sp, long? ScopeId, TItem[] Items)
			{
				if (Items.Length == 0)
					return;
				var keys = new List<TKey>();
				foreach (var item in Items)
					CollectIdents(item, keys);
				if (keys.Count== 0)
					return;

				var manager = sp.Resolve<TManager>();
				if (manager == null)
					return;

				Dictionary<TKey,TEntity> entities;
				var mel = manager as IEntityBatchLoadable<ObjectKey<TKey>, TEntity>;
				if (mel != null)
					entities = (await mel.BatchGetAsync(
						keys.Where(id=>!id.IsDefault()).Distinct().Select(id => new ObjectKey<TKey> { Id = id }).ToArray(),
						Properties
						)).ToDictionary(e => e.Id);
				else
				{
					entities = new Dictionary<TKey, TEntity>();
					foreach (var id in keys.Where(i=>!i.IsDefault()).Distinct())
					{
						var ins = await manager.GetAsync(new ObjectKey<TKey> { Id = id });
						if (ins != null)
							entities[id] = ins;
					}
				}
				if (entities.Count == 0)
					return;
				var ffl = FillFields.Length;
				var il = Items.Length;
				for (var i = 0; i < il; i++)
				{
					var item = Items[i];
					for (var j = 0; j < ffl; j++)
					{
						var k = keys[i * ffl + j];
						if (k.IsDefault())
							continue;
						if (!entities.TryGetValue(k, out var e))
							continue;

						FillFields[j](item, e);
					}
				}
			}
		}
		class Filler { }

		class Filler<TItem> : Filler
		{
			EntityFiller<TItem>[] EntityFillers { get; }
			string GetEntityTitleField(Type Type)
			{
				var props = Type.AllPublicInstanceProperties();
				var dta = props.FirstOrDefault(t => t.IsDefined(typeof(EntityTitleAttribute)));
				if (dta != null) return dta.Name;

				dta = props.FirstOrDefault(t => t.Name == "Title" || t.Name == "Name");
				if (dta != null)
					return dta.Name;

				throw new InvalidOperationException("找不到类型标题:" + Type.FullName);
			}
			public Filler(IEntityMetadataCollection MetadataCollection)
			{
				var AllProps = typeof(TItem).AllPublicInstanceProperties();


				PropertyInfo findSrcProp(Type type,string name,PropertyInfo dstProp)
				{
					var re = type.GetProperty(name);
					if (re == null) throw new NotSupportedException($"实体类型{type}中找不到属性{name}");
					if (!dstProp.PropertyType.IsAssignableFrom(re.PropertyType) && dstProp.PropertyType!=typeof(string))
						throw new InvalidOperationException($"源属性{re.DeclaringType.ShortName()}.{re.Name}类型{re.PropertyType.ShortName()}和目标属性{dstProp.DeclaringType.ShortName()}.{dstProp.Name}类型{dstProp.PropertyType.ShortName()}不兼容");
					return re;
				}

				var settings = (from prop in AllProps
								let ei = prop.GetCustomAttribute<EntityIdentAttribute>()
								where ei != null && ei.EntityType!=null
								let meta = MetadataCollection.FindByEntityType(ei.EntityType) ?? 
											throw new NotSupportedException($"未找到类型{ei.EntityType}对于的实体管理器")
								let eprops = (from p in AllProps
											  let pa = p.GetCustomAttribute<FromEntityPropertyAttribute>() ??
														(ei.NameField==p.Name?new FromEntityPropertyAttribute(
															prop.Name,
															GetEntityTitleField(ei.EntityType)
															):null)
											  where pa != null && pa.IdentField == prop.Name											  
											 let srcProp=findSrcProp(meta.EntityDetailType,pa.Property, p)
											select (dstProp: p, srcProp: srcProp)
											).ToArray()
								where eprops.Length > 0
								group (IdProp: prop, EntityType: meta.EntityDetailType, ManagerType:meta.EntityManagerType, ValueProps: eprops) by meta.EntityDetailType
								into g
								select g
					).ToArray();

				EntityFillers = settings.Select(
					g =>
					{
						var idType = g.First().IdProp.PropertyType;
						var realIdType = idType.GetGenericArgumentTypeAsNullable() ?? idType;

						return (EntityFiller<TItem>)Activator.CreateInstance(
							typeof(EntityFiller<,,,>).MakeGenericType(
								realIdType,
								g.Key,
								g.First().ManagerType,
								typeof(TItem)
								),
							g.ToArray()
							);
					}
					).ToArray();
			}

			public async Task Fill(IServiceProvider sp, long? ServiceScopeId, TItem[] Items)
			{
				foreach (var f in EntityFillers)
					await f.Fill(sp, ServiceScopeId, Items);
			}
		}

		IServiceProvider ServiceProvider { get; }
		IEntityMetadataCollection MetadataCollection { get; }
		static System.Collections.Concurrent.ConcurrentDictionary<Type, Filler> Fillers { get; } = 
			new System.Collections.Concurrent.ConcurrentDictionary<Type, Filler>();


		public EntityPropertyFiller(
			IServiceProvider ServiceProvider, 
			IEntityMetadataCollection MetadataCollection
			)
		{
			this.ServiceProvider = ServiceProvider;
			this.MetadataCollection = MetadataCollection;
		}
		
		public async Task Fill<TItem>(long? ServiceScopeId, TItem[] Items)
		{
			var key = typeof(TItem);
			if(!Fillers.TryGetValue(key, out var fs))
				fs = Fillers.GetOrAdd(key, new Filler<TItem>(MetadataCollection));
			await ((Filler<TItem>)fs).Fill(ServiceProvider,ServiceScopeId, Items);
		}
	}
}

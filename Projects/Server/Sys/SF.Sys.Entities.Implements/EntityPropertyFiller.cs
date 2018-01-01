#region Apache License Version 2.0
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Sys.Entities
{
	public class EntityPropertyFiller : IEntityPropertyFiller
	{
		IServiceProvider ServiceProvider { get; }
		public EntityPropertyFiller(
			IServiceProvider ServiceProvider
			)
		{
			this.ServiceProvider = ServiceProvider;
		}

		interface IFiller<TItem>
		{
			Task Fill(
				IServiceProvider ServiceProvider,
				TItem[] Items
				);
		}
		class Filler<TKey, TEntity, TItem> :
			IFiller<TItem>
			where TEntity:IEntityWithId<TKey>
			where TKey:IEquatable<TKey>
		{
			Action<TItem, TEntity>[] FillFields { get; }
			Action<TItem, List<TKey>> CollectIdents { get; }
			string[] Properties { get; }
			bool FillRequired { get; }

			public Filler((PropertyInfo IdProp,Type EntityType,(PropertyInfo dstProp,string srcProp)[] ValueProps)[] settings)
			{
				var varName = Expression.Variable(typeof(string));
				var ArgItem = Expression.Parameter(typeof(TItem));
				var ArgNames = Expression.Parameter(typeof(IReadOnlyList<string>));

				
				FillRequired = settings.Length > 0;
				if (!FillRequired)
					return;

				Properties = settings
					.SelectMany(p => p.ValueProps)
					.Select(p => p.srcProp)
					.Distinct()
					.ToArray();

				var argItem = Expression.Parameter(typeof(TItem));
				var argList = Expression.Parameter(typeof(List<TKey>));

				CollectIdents = Expression.Lambda<Action<TItem, List<TKey>>>(
					Expression.Block(
						settings.Select(s =>
							argList.CallMethod(
								"Add",
								Expression.Property(
									argItem,
									s.IdProp
									)
								)
							)
						),
						argItem,
						argList
					).Compile();

				var argEntity = Expression.Parameter(typeof(TEntity));
				FillFields = (from setting in settings
							  select BlockExpression.Block(
									  setting.ValueProps.Select(ep =>
										  argItem.SetProperty(
											  ep.dstProp,
											  argEntity.GetMember(

												  ep.srcProp
											  )
										  )
									  )
								  ).Compile<Action<TItem, TEntity>>(
								  argItem,
								  argEntity
								  )
							).ToArray();

			}
			

			public async Task Fill(
				IServiceProvider ServiceProvider,
				TItem[] Items
				)
			{
				var keys = new List<TKey>();
				foreach (var item in Items)
					CollectIdents(item, keys);

				var entities = (await ServiceProvider.Resolve<IEntityBatchLoadable<TKey, TEntity>>()
					.BatchGetAsync(
					keys.Where(k => !k.IsDefault()).Distinct().ToArray(),
					Properties
					)).ToDictionary(e => e.Id);

				var ffl = FillFields.Length;
				var il = Items.Length;
				for(var i = 0; i < il; i++)
				{
					var item = Items[i];
					for(var j = 0; j < il; j++)
					{
						var k = keys[i * il + j];
						if (k.IsDefault())
							continue;
						if (!entities.TryGetValue(k, out var e))
							continue;

						FillFields[j](item, e);
					}
				}
			}
		}

		static class Filler<TItem>
		{
			public static IFiller<TItem>[] EntityFillers { get; }
			static Filler()
			{
				var AllProps = typeof(TItem).AllPublicInstanceProperties();

				var settings = (from prop in AllProps
					let ei = prop.GetCustomAttribute<EntityIdentAttribute>()
					where ei != null 
					let nameProp = ei.NameField == null ?
									null :
									AllProps.FirstOrDefault(p => p.Name == ei.NameField)
									.IsNotNull(() => $"实体{typeof(TItem)}中找不到名字属性{ei.NameField}")
									.Assert(p => p.PropertyType == typeof(string), p => $"实体{typeof(TItem)}的名称字段{ei.NameField}不是字符串类型")
					let eprops = (from p in AllProps
									let pa = p.GetCustomAttribute<FromEntityPropertyAttribute>()
									where pa != null && pa.IdentField == prop.Name
									select (dstProp: p, srcProp: pa.Property)
								).WithFirst((dstProp: nameProp, srcProp: "Name"))
								.Where(p => p.dstProp != null).ToArray()
					where eprops.Length > 0
					
					group(IdProp: prop, EntityType: ei.EntityType, ValueProps: eprops) by ei.EntityType
					into g 
					select g
					).ToArray();

				EntityFillers = settings.Select(
					g => (IFiller<TItem>)Activator.CreateInstance(
						typeof(Filler<,,>).MakeGenericType(
							g.First().IdProp.PropertyType,
							g.Key,
							typeof(TItem)
							),
						g.ToArray()
						)
					).ToArray();
			}

			public static async Task Fill(IServiceProvider sp,TItem[] Items)
			{
				foreach (var f in EntityFillers)
					await f.Fill(sp, Items);
			}
		}
		public async Task Fill<TItem>( TItem[] Items)
		{
			await Filler<TItem>.Fill(ServiceProvider, Items);
		}
	}
}

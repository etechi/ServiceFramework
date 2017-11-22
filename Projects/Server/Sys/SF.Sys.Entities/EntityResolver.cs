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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using SF.Sys.Collections.Generic;
using SF.Sys.Reflection;
using SF.Sys.Entities.Annotations;
using SF.Sys.Linq.Expressions;
using SF.Sys.Annotations;

namespace SF.Sys.Entities
{
    public static class EntityResolver
    {
        public static async Task<IEntityReference> Resolve(this IEntityReferenceResolver Resolver, string Ident,long? ScopeId)
        {
			if (Ident.IsNullOrWhiteSpace())
				return null;

			var ei = EntityIdents.Parse(Ident);
			if (ei.Type == "i")
			{
				if (ei.KeyCount != 2)
					throw new NotSupportedException($"实体ID格式错误:{Ident}");
				var re = await Resolver.Resolve(
					Convert.ToInt64(ei.Keys[0]),
					new[] { Convert.ToString(ei.Keys[1]) }
					);
				return re == null ? null : re.Length == 0 ? null : re[0];
			}
			else
			{
				if (ei.KeyCount != 1)
					throw new NotSupportedException($"实体ID格式错误:{Ident}");
				var re = await Resolver.Resolve(
					ScopeId,
					ei.Type,
					new[] { Convert.ToString(ei.Keys[0]) }
					);
				return re == null ? null : re.Length == 0 ? null : re[0];
			}
		}
        public static Task<IEntityReference[]> Resolve(this IEntityReferenceResolver Resolver,long? ScopeId,params string[] Idents)
            =>Resolve(Resolver, ScopeId, (IEnumerable<string>)Idents);
		
		public static async Task<IEntityReference[]> Resolve(this IEntityReferenceResolver Resolver, long? ScopeId,IEnumerable<string> Idents)
        {
			var idGroups = Idents
				.Where(id => !string.IsNullOrWhiteSpace(id))
				.Distinct()
				.Select(id => EntityIdents.Parse(id))
				.GroupBy(id => id.Type);

            var re = new List<IEntityReference>();
			foreach (var g in idGroups)
			{
				if(g.Key=="i")
				{
					if(g.Any(i=>i.KeyCount!=2))
						throw new NotSupportedException($"实体ID格式错误:{g.Where(i => i.KeyCount != 2).Select(i=>i.ToIdentString())}");
					foreach (var gi in g.GroupBy(i => i.Keys[0]))
						re.AddRange(
							(await Resolver.Resolve(
								Convert.ToInt64(gi.Key),
								gi.Select(gii => Convert.ToString(gii.Keys[1]))
						   )).Where(a => a != null)
						   );
				}
				else
					re.AddRange(await Resolver.Resolve(ScopeId, g.Key, g.Select(gi=>Convert.ToString(gi.Keys[0]))));
			}
            return re.ToArray();
        }


		public static async Task<Dictionary<string, IEntityReference>> ResolveToDictionary(
			this IEntityReferenceResolver Resolver,
			long? ScopeId,
			IEnumerable<string> Idents)
		{
			var re = await Resolver.Resolve(ScopeId,Idents);
			return re.ToDictionary(o => o.Id);
		}
	

		public static Task<IEntityReference[]> ResolveWithOrder(this IEntityReferenceResolver Resolver,long? ScopeId ,params string[] Idents)
            => ResolveWithOrder(Resolver, ScopeId, (IEnumerable<string>)Idents);

        public static async Task<IEntityReference[]> ResolveWithOrder(this IEntityReferenceResolver Resolver, long? ScopeId, IEnumerable<string> Idents)
        {
            var dic = await Resolver.ResolveToDictionary(ScopeId, Idents);
            var re = new List<IEntityReference>();
			foreach (var id in Idents)
			{
				if (string.IsNullOrWhiteSpace(id))
					re.Add(null);
				else {
					var v = dic.Get(id);
					if (v!=null)
						re.Add(v);
				}
			}
			return re.ToArray();
        }


        public static async Task Fill<TItem,TResult>(
            this IEntityReferenceResolver Resolver,
			long? ScopeId,
            IEnumerable<TItem> items,
            Func<TItem, IEnumerable<(Type,string)>> IdSelector,
            Func<IEntityReference, TResult> ResultSelector,
            Action<TItem, IReadOnlyList<TResult>> action
            )
        {
            var keys = items.Select(it => new {
				item = it,
				keys = IdSelector(it)
					.Select(i=>
					{
						if (i.Item2 == null)
							return null;
						if (i.Item1 == null)
							return i.Item2;
						var e = Resolver.FindEntityIdent(i.Item1);
						if (e == null)
							return null;
						return e + "-" + i.Item2;
					})
					.ToArray()
			}).ToArray();
            var dic = await Resolver.ResolveToDictionary(
				ScopeId,
				keys.SelectMany(k => k.keys.Where(ki=>ki!=null))
				);
            var rs = new List<TResult>();
            foreach (var r in keys)
            {
                rs.Clear();
                foreach (var k in r.keys)
                    if (string.IsNullOrWhiteSpace(k))
                        rs.Add(default(TResult));
                    else
                    {
                        var o=dic.Get(k);
                        rs.Add(o == null ? default(TResult) : ResultSelector(o));
                    }
                action(r.item, rs);
            }
        }
        public static Task Fill<TItem>(
           this IEntityReferenceResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, IEnumerable<string>> IdSelector,
           Func<IEntityReference, IEntityReference> ResultSelector,
           Action<TItem, IReadOnlyList<IEntityReference>> action
           ) =>
            Resolver.Fill(items, IdSelector, i => i, action);

        public static async Task Fill<TItem,TResult>(
            this IEntityReferenceResolver Resolver,
  			long? ScopeId,
			IEnumerable<TItem> items,
            Func<TItem, string> IdSelector,
            Func<IEntityReference,TResult> ResultSelector,
            Action<TItem, TResult> action            
            )
        {
            var keys = items.Select(it => new { item = it, key = IdSelector(it) }).ToArray();
            var objs = await Resolver.ResolveToDictionary(ScopeId,keys.Select(k => k.key));
            foreach (var r in keys)
            {
                TResult result;
                if (string.IsNullOrWhiteSpace(r.key))
                    result = default(TResult);
                else
                {
					var os=objs.Get(r.key);
                    result = os == null ? default(TResult) : ResultSelector(os);
                }
                action(r.item, result);
            }
        }
        public static Task Fill<TItem>(
           this IEntityReferenceResolver Resolver,
   			long? ScopeId,
			IEnumerable<TItem> items,
           Func<TItem, string> IdSelector,
           Action<TItem, IEntityReference> action
           ) => Resolver.Fill(ScopeId, items, IdSelector, i => i, action);

        public static Task Fill<TItem>(
			this IEntityReferenceResolver Resolver,
			long? ScopeId,
			IEnumerable<TItem> items,
			Func<TItem, IEnumerable<(Type,string)>> IdSelector,
			Action<TItem, IReadOnlyList<string>> action
           )
            => Resolver.Fill(ScopeId,items, IdSelector, i => i.Name, action);

        public static Task Fill<TItem>(
           this IEntityReferenceResolver Resolver,
 			long? ScopeId,
		  IEnumerable<TItem> items,
           Func<TItem, string> IdSelector,
           Action<TItem, string> action
           )
            => Resolver.Fill(ScopeId, items, IdSelector, i => i.Name, action);

		class Filler {
			protected static (Type,string) ToIdent<T>(Type Type,T Id,string Name)
			{
				if (Id.IsDefault() || Name!=null)
					return (Type,null);
				return (Type,Id.ToString());
			}
			protected static MethodInfo ToIdentMethod { get; } =
				typeof(Filler).GetMethod(
					nameof(ToIdent), 
					BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod
					);
			protected static string GetItemName(IReadOnlyList<string> list, int index) => list[index];

		}

		class Filler<TItem>:Filler
		{
			static ParameterExpression ArgItem { get; } = Expression.Parameter(typeof(TItem));
			static ParameterExpression ArgNames { get; } = Expression.Parameter(typeof(IReadOnlyList<string>));
			static PropertyInfo[] AllProps { get; } = typeof(TItem).AllPublicInstanceProperties();

			static (PropertyInfo id, PropertyInfo name, EntityIdentAttribute attr)[] PropPairs { get; } = 
				(from prop in AllProps
				 let ei = prop.GetCustomAttribute<EntityIdentAttribute>()
				 where ei != null && ei.NameField != null
				 let nameProp = AllProps.FirstOrDefault(p => p.Name == ei.NameField)
					.IsNotNull(()=>$"实体{typeof(TItem)}中找不到名字属性{ei.NameField}")
					.Assert(p=>p.PropertyType==typeof(string),p=> $"实体{typeof(TItem)}的名称字段{ei.NameField}不是字符串类型")
				 select (prop, nameProp,ei)
				 ).ToArray();
			static bool FillRequired { get; } = PropPairs.Length > 0;

			static Func<TItem, (Type,string)[]> GetIdents { get; } =
				Expression.Lambda<Func<TItem, (Type,string)[]>>(
					Expression.NewArrayInit(
						typeof((Type,string)),
						PropPairs.Select(p=>
						Expression.Call(
							 null,
							 ToIdentMethod.MakeGenericMethod(p.id.PropertyType),
							 Expression.Constant(p.attr.EntityType),
							 Expression.Property(ArgItem, p.id),
							 Expression.Property(ArgItem, p.name)
							 )
						).ToArray()
						),
					ArgItem	
					).Compile();

			static ParameterExpression varName { get; } = Expression.Variable(typeof(string));
			static Action<TItem, IReadOnlyList<string>> FillIdents
			{
				get
				{
					var exprs = new List<Expression>();
					for (var i = 0; i < PropPairs.Length; i++)
					{
						var p = PropPairs[i];
						exprs.Add(
							varName.Assign(
								Expression.Call(
									null,
									typeof(Filler).GetMethod(
										nameof(GetItemName),
										BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod),
									ArgNames,
									Expression.Constant(i)
									)
								)
							);
						exprs.Add(
							Expression.NotEqual(varName, Expression.Constant(null, typeof(string))).Then(
								ArgItem.SetProperty(p.name, varName)
								)
							);
					}
					return Expression.Lambda<Action<TItem, IReadOnlyList<string>>>(
						Expression.Block(
							new[] { varName },
							exprs
						),
						ArgItem,
						ArgNames
					).Compile();
				}
			}
			public static async Task Fill(IEntityReferenceResolver Resolver,long? ScopeId,IEnumerable<TItem> items)
			{
				if(FillRequired)
					await Resolver.Fill(
						ScopeId,
						items,
						GetIdents,
						FillIdents
						);
			}
		}
		public static Task Fill<TItem>(
			this IEntityReferenceResolver Resolver,
			long? ScopeId,
			IEnumerable<TItem> items
			)
		{
			return Filler<TItem>.Fill(Resolver, ScopeId, items);
		}
    }
   
}

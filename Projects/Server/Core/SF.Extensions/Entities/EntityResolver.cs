using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using System.Linq.Expressions;

namespace SF.Entities
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
            Func<TItem, IEnumerable<string>> IdSelector,
            Func<IEntityReference, TResult> ResultSelector,
            Action<TItem, IReadOnlyList<TResult>> action
            )
        {
            var keys = items.Select(it => new { item = it, keys = IdSelector(it).ToArray() }).ToArray();
            var dic = await Resolver.ResolveToDictionary(ScopeId,keys.SelectMany(k => k.keys));
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
			Func<TItem, IEnumerable<string>> IdSelector,
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
			protected static string ToIdent<T>(string Type, T Id)
			{
				return Type + "-" + Id.ToString();
			}
			protected static MethodInfo ToIdentMethod { get; } = typeof(Filler).GetMethod(nameof(ToIdent), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod);
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
				 let nameProp = AllProps.FirstOrDefault(p => p.Name == ei.NameField) ?? throw new ArgumentException($"实体{typeof(TItem)}没有指定的字段{ei.NameField}")
				 select (prop, nameProp,ei)
				 ).ToArray();
			static bool FillRequired { get; } = PropPairs.Length > 0;

			static Func<TItem, string[]> GetIdents { get; } =
				Expression.Lambda<Func<TItem, string[]>>(
					Expression.NewArrayInit(
						typeof(string),
						PropPairs.Select(p=>
						Expression.Call(
							 null,
							 ToIdentMethod.MakeGenericMethod(p.id.PropertyType),
							 Expression.Constant(p.attr.EntityType.FullName),
							 Expression.Property(ArgItem, p.id)
							 )
						).ToArray()
						),
					ArgItem
					).Compile();

			static Action<TItem, IReadOnlyList<string>> FillIdents { get; } =
				Expression.Lambda<Action<TItem, IReadOnlyList<string>>>(
					Expression.Block(
						PropPairs.Select((p, i) =>
						Expression.Call(
							ArgItem,
							p.name.GetSetMethod(),
							Expression.Call(
								null,
								typeof(Filler).GetMethod(nameof(GetItemName), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod),
								ArgNames,
								Expression.Constant(i)
								)
						)
					)
					),
					ArgItem,
					ArgNames
					).Compile();

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

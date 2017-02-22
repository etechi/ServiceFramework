using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SF.Data.Entity
{
    public static class EntityResolver
    {
        public static async Task<IDataEntity> Resolve(this IDataEntityResolver Resolver, string Ident)
        {
            if (string.IsNullOrWhiteSpace(Ident))
                return null;
            var id = Ident.Split2('-');
            var re = await Resolver.Resolve(id.Item1, new string[]{ id.Item2});
            return re == null ? null : re.Length == 0 ? null : re[0];
        }
        public static Task<IDataEntity[]> Resolve(this IDataEntityResolver Resolver, params string[] Idents)
            =>Resolve(Resolver, (IEnumerable<string>)Idents);
        public static async Task<IDataEntity[]> Resolve(this IDataEntityResolver Resolver, IEnumerable<string> Idents)
        {
			var idGroups = Idents
				.Where(id => !string.IsNullOrWhiteSpace(id))
				.Distinct()
				.Select(id => id.Split2('-'))
                .GroupBy(id => id.Item1, g => g.Item2);

            var re = new List<IDataEntity>();
            foreach(var g in idGroups)
                re.AddRange(await Resolver.Resolve(g.Key, g.ToArray()));
            return re.ToArray();
        }


		public static async Task<Dictionary<string, IDataEntity>> ResolveToDictionary(
			this IDataEntityResolver Resolver,
			IEnumerable<string> Idents)
		{
			var re = await Resolver.Resolve(Idents);
			return re.ToDictionary(o => o.Ident);
		}

        public static Task<IDataEntity[]> ResolveWithOrder(this IDataEntityResolver Resolver, params string[] Idents)
            => ResolveWithOrder(Resolver, (IEnumerable<string>)Idents);

        public static async Task<IDataEntity[]> ResolveWithOrder(this IDataEntityResolver Resolver, IEnumerable<string> Idents)
        {
            var dic = await Resolver.ResolveToDictionary(Idents);
            var re = new List<IDataEntity>();
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
            this IDataEntityResolver Resolver,
            IEnumerable<TItem> items,
            Func<TItem, IEnumerable<string>> IdSelector,
            Func<IDataEntity, TResult> ResultSelector,
            Action<TItem, IReadOnlyList<TResult>> action
            )
        {
            var keys = items.Select(it => new { item = it, keys = IdSelector(it).ToArray() }).ToArray();
            var dic = await Resolver.ResolveToDictionary(keys.SelectMany(k => k.keys));
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
           this IDataEntityResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, IEnumerable<string>> IdSelector,
           Func<IDataEntity, IDataEntity> ResultSelector,
           Action<TItem, IReadOnlyList<IDataEntity>> action
           ) =>
            Resolver.Fill(items, IdSelector, i => i, action);

        public static async Task Fill<TItem,TResult>(
            this IDataEntityResolver Resolver,
            IEnumerable<TItem> items,
            Func<TItem, string> IdSelector,
            Func<IDataEntity,TResult> ResultSelector,
            Action<TItem, TResult> action            
            )
        {
            var keys = items.Select(it => new { item = it, key = IdSelector(it) }).ToArray();
            var objs = await Resolver.ResolveToDictionary(keys.Select(k => k.key));
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
           this IDataEntityResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, string> IdSelector,
           Action<TItem, IDataEntity> action
           ) => Resolver.Fill(items, IdSelector, i => i, action);

        public static Task Fill<TItem>(
           this IDataEntityResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, IEnumerable<string>> IdSelector,
           Action<TItem, IReadOnlyList<string>> action
           )
            => Resolver.Fill(items, IdSelector, i => i.Name, action);

        public static Task Fill<TItem>(
           this IDataEntityResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, string> IdSelector,
           Action<TItem, string> action
           )
            => Resolver.Fill(items, IdSelector, i => i.Name, action);
    }
   
}

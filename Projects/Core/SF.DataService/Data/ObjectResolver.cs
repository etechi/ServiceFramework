using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Reflection;
using System.Reflection;
namespace SF.Data
{
    
    public static class ObjectIdent
    {
        public static string Build(string Type,params object[] Idents)
        {
            return Type + "-" + string.Join("-", Idents);
        }
        public static string Build<T>(string Type, T Ident)
        {
            return Type + "-" + Convert.ToString(Ident);
        }
        public static KeyValuePair<string, string[]> Parse(string Ident)
        {
            var i = Ident.IndexOf('-');
            if (i == -1)
                throw new ArgumentException();
            var type = Ident.Substring(0, i);
            var keys = Ident.Substring(i + 1).Split('-');
            if (string.IsNullOrWhiteSpace(type) || keys.Length==0)
                throw new ArgumentException("对象ID格式错误：" + Ident);
            return new KeyValuePair<string, string[]>(type,keys);
        }
        public static KeyValuePair<string, T> ParseSingleKey<T>(string Ident)
        {
            var re = Parse(Ident);
            if (re.Value.Length != 1)
                throw new ArgumentException("对象ID包含多个主键字段:" + Ident);
            var id = (T)Convert.ChangeType(re.Value[0],typeof(T));
            return new KeyValuePair<string, T>(re.Key, id);
        }
        public static T ParseSingleKey<T>(string Ident, string Type)
        {
            var p = ParseSingleKey<T>(Ident);
            if (p.Key != Type)
                throw new ArgumentException($"数据对象类型错误：希望类型：{Type} 实际类型:{p.Key}");
            return p.Value;
        }
        public static string[] Parse(string Ident, string Type)
        {
            var p = Parse(Ident);
            if (p.Key != Type)
                throw new ArgumentException($"数据对象类型错误：希望类型：{Type} 实际类型:{p.Key}");
            return p.Value;
        }
    }
    public static class DataObjectResolver
    {
        public static async Task<IDataObject> Resolve(this IDataObjectResolver Resolver, string Ident)
        {
            if (string.IsNullOrWhiteSpace(Ident))
                return null;
            var id = ObjectIdent.Parse(Ident);
            var re = await Resolver.Resolve(id.Key, new string[][] { id.Value });
            return re == null ? null : re.Length == 0 ? null : re[0];
        }
        public static Task<IDataObject[]> Resolve(this IDataObjectResolver Resolver, params string[] Idents)
            =>Resolve(Resolver, (IEnumerable<string>)Idents);
        public static async Task<IDataObject[]> Resolve(this IDataObjectResolver Resolver, IEnumerable<string> Idents)
        {
            var idGroups = Idents
                .Where(id=>!string.IsNullOrWhiteSpace(id))
                .Distinct()
                .Select(id => ObjectIdent.Parse(id))
                .GroupBy(id => id.Key, g => g.Value);

            var re = new List<IDataObject>();
            foreach(var g in idGroups)
                re.AddRange(await Resolver.Resolve(g.Key, g.ToArray()));
            return re.ToArray();
        }
        

        public static async Task<Dictionary<string, IDataObject>> ResolveToDictionary(
            this IDataObjectResolver Resolver,
            IEnumerable<string> Idents)
            => (await Resolver.Resolve(Idents)).ToDictionary(o => o.Ident);

        public static Task<IDataObject[]> ResolveWithOrder(this IDataObjectResolver Resolver, params string[] Idents)
            => ResolveWithOrder(Resolver, (IEnumerable<string>)Idents);

        public static async Task<IDataObject[]> ResolveWithOrder(this IDataObjectResolver Resolver, IEnumerable<string> Idents)
        {
            var dic = await Resolver.ResolveToDictionary(Idents);
            var re = new List<IDataObject>();
			foreach (var id in Idents)
			{
				if (string.IsNullOrWhiteSpace(id))
					re.Add(null);
				else if (dic.TryGetValue(id, out var v))
					re.Add(v);
			}
			return re.ToArray();
        }


        public static async Task Fill<TItem,TResult>(
            this IDataObjectResolver Resolver,
            IEnumerable<TItem> items,
            Func<TItem, IEnumerable<string>> IdSelector,
            Func<IDataObject, TResult> ResultSelector,
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
                        dic.TryGetValue(k ,out var o );
                        rs.Add(o == null ? default(TResult) : ResultSelector(o));
                    }
                action(r.item, rs);
            }
        }
        public static Task Fill<TItem>(
           this IDataObjectResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, IEnumerable<string>> IdSelector,
           Func<IDataObject, IDataObject> ResultSelector,
           Action<TItem, IReadOnlyList<IDataObject>> action
           ) =>
            Resolver.Fill(items, IdSelector, i => i, action);

        public static async Task Fill<TItem,TResult>(
            this IDataObjectResolver Resolver,
            IEnumerable<TItem> items,
            Func<TItem, string> IdSelector,
            Func<IDataObject,TResult> ResultSelector,
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
                    objs.TryGetValue(r.key,out var os);
                    result = os == null ? default(TResult) : ResultSelector(os);
                }
                action(r.item, result);
            }
        }
        public static Task Fill<TItem>(
           this IDataObjectResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, string> IdSelector,
           Action<TItem, IDataObject> action
           ) => Resolver.Fill(items, IdSelector, i => i, action);

        public static Task Fill<TItem>(
           this IDataObjectResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, IEnumerable<string>> IdSelector,
           Action<TItem, IReadOnlyList<string>> action
           )
            => Resolver.Fill(items, IdSelector, i => i.Name, action);

        public static Task Fill<TItem>(
           this IDataObjectResolver Resolver,
           IEnumerable<TItem> items,
           Func<TItem, string> IdSelector,
           Action<TItem, string> action
           )
            => Resolver.Fill(items, IdSelector, i => i.Name, action);
    }
   
   
    public static class DataObjectLoader
    {
        public static IEnumerable<KeyValuePair<string,Type>> FindDataObjectLoaders(params System.Reflection.Assembly[] Assemblies)
        {
            var re = new List<KeyValuePair<string, Type>>();
            foreach(var ass in Assemblies)
            {
                try
                {
                    re.AddRange(
                        from type in ass.GetTypes()
                        where !type.IsGenericDefinition()
                        from oattr in type.GetCustomAttributes(true).Where(a=>a is DataObjectLoaderAttribute)
                        let attr = oattr as DataObjectLoaderAttribute
                        where attr != null && type.GetInterface(typeof(IDataObjectLoader).FullName) == typeof(IDataObjectLoader)
                        select new KeyValuePair<string, Type>(attr.Type, type)
                        );
                }
                catch { }
            }
            return re;
        }
        public static async Task<IDataObject[]> Load<T,R>(
            string[][] Keys,
            Func<string[],T> IdParser,
            Func<T, Task<R>> SingleLoad,
            Func<T[], Task<R[]>> MultipleLoad
            )
            where R: class,IDataObject
        {
            if (Keys.Length == 0)
                return Array.Empty<IDataObject>();
            if (Keys.Length == 1)
            {
                var re = await SingleLoad(IdParser(Keys[0]));
                if (re == null)
                    return Array.Empty<IDataObject>();
                return new[] { (IDataObject)re };
            }
            var ids = Keys.Select(k => IdParser(k)).ToArray();
            var res = await MultipleLoad(ids);
            return res;
        }
    }


    public class DefaultDataObjectResolver : IDataObjectResolver
    {
        Func<string,IDataObjectLoader> LoaderResolver { get; }
        public DefaultDataObjectResolver(Func<string, IDataObjectLoader> LoaderResolver)
        {
            this.LoaderResolver = LoaderResolver;
        }

        public Task<IDataObject[]> Resolve(string Type, string[][] Keys)
        {
            var loader = LoaderResolver(Type);
            if (loader == null)
                return Task.FromResult(Array.Empty<IDataObject>());
                //throw new ArgumentException("不支持指定数据类：" + Type);
            return loader.Load(Type,Keys);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public static class DataContextExtension
	{
		//public static ObjectContext GetObjectContext(this DbContext ctx)
		//{
		//	return ((IObjectContextAdapter)ctx).ObjectContext;
		//}
		//public static object[] GetIdentArray(this DbContext ctx, object item)
		//{
		//	var re = GetIdent(ctx, item);
		//	if (re == null) return null;
		//	return re.Select(p => p.Value).ToArray();
		//}
		//public static IEnumerable<KeyValuePair<string,object>> GetIdent(this DbContext ctx, object item)
		//{
		//	var e = ctx.Entry(item);

		//	var oc = GetObjectContext(ctx);
		//	var type = item.GetType();
		//	do
		//	{
		//		var key = oc.CreateEntityKey(type.Name, item);
		//		if (key != null)
		//			return key.EntityKeyValues.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value));
		//		type = type.BaseType;
		//	}
		//	while (type != typeof(object) && type != typeof(MarshalByRefObject));
		//	return null;
		//}

		//public static bool IsCollectionLoaded<T, C>(this DbContext context, T user, System.Linq.Expressions.Expression<Func<T, ICollection<C>>> collection)
		//	where T : class
		//	where C : class
		//{
		//	return context.Entry(user).Collection(collection).IsLoaded;
		//}
		//public static async Task LoadCollection<T, C>(this DbContext context, T entity, System.Linq.Expressions.Expression<Func<T, ICollection<C>>> collection)
		//	where T : class
		//	where C : class
		//{
		//	var col = context.Entry(entity).Collection(collection);
		//	if (col.IsLoaded) return;
		//	await col.LoadAsync();
		//	col.IsLoaded = true;
		//}
	}
}

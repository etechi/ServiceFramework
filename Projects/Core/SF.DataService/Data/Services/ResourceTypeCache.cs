using System;
using System.Linq;
using SF.Reflection;

namespace SF.Data.Services
{
	static class ResourceTypeCache
	{
		static System.Collections.Concurrent.ConcurrentDictionary<Type, string> Cache { get; } = new System.Collections.Concurrent.ConcurrentDictionary<Type, string>();
		public static string GetResourceType(Type type) { 
			string rt;
			if (Cache.TryGetValue(type, out rt))
				return rt;

			rt = null;
			var dl = type.GetCustomAttributes<DataObjectLoaderAttribute>(true).ToArray();
			if (dl != null)
			{
				if (dl.Length == 1)
					rt = ((DataObjectLoaderAttribute)dl[0]).Type;
				else
				{
					dl = type.GetCustomAttributes<DataObjectLoaderAttribute>(false).ToArray();
					if (dl != null && dl.Length == 1)
						rt = ((DataObjectLoaderAttribute)dl[0]).Type;
				}
			}
			rt = Cache.GetOrAdd(type, rt);
			if (rt == null)
				throw new NotSupportedException(type.FullName + "不支持ResourceType");
			return rt;
		}
	}
    
   
}
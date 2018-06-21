using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Sys.Comments;
namespace SF.Utils.TableExports
{

	public class EnumFormat : IFormat
    {
        Dictionary<string,string> Names { get; }
        public EnumFormat(Type type)
        {
            Names = type
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                    .Select(p => new
                    {
                        name = p.Name,
                        title = p.Comment().Title
                    })
                    .ToDictionary(p => p.name, p => p.title);
        }
             
        public object Format(object value)
        {
            var str = Convert.ToString(value);
            if (str == null) return string.Empty;
            string re;
            return Names.TryGetValue(str, out re) ? re : str;
        }
    }
}

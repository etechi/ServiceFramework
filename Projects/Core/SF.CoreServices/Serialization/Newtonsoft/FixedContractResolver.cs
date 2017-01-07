
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Reflection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace SF.Serialization.Newtonsoft
{
	public class FixedContractResolver : DefaultContractResolver
	{
		protected FixedContractResolver() { }
		public static FixedContractResolver Instance { get; } = new FixedContractResolver();
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var p = base.CreateProperty(member, memberSerialization);
			if (member is PropertyInfo pi)
			{
				if (pi.PropertyType.IsEnumType())
					p.DefaultValue = Enum.ToObject(pi.PropertyType, -1);
				else if (pi.Name == "TypeId" && pi.DeclaringType == typeof(Attribute))
					p.Ignored = true;
			}
			return p;
		}
	}
}

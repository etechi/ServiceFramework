using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;

namespace SF.Entities.AutoEntityProvider.Internals.ValueTypes
{
	public class JsonDataValueTypeProvider : IValueTypeProvider
	{
		class JsonDataValueType : IValueType
		{
			public JsonDataValueType(Type Type)
			{
				this.SysType = Type;
				this.Attributes = EntityAttribute.GetAttributes(Type).ToArray();
			}
			public Type SysType { get; }

			public string Name => SysType.FullName;

			public IReadOnlyList<IAttribute> Attributes { get; }
		}
		public JsonDataValueTypeProvider()
		{
		}

		public int Priority => -1;
	
		public IValueType DetectValueType(string TypeName, string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes)
		{
			if (Attributes.Any(a => a.Name == typeof(JsonDataAttribute).FullName))
				return new JsonDataValueType(SystemType);
			return null;
		}


	}
}

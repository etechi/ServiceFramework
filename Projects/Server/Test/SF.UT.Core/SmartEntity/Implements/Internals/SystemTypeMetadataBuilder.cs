using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
namespace SF.Entities.Smart.Internals
{
	public class SystemTypeMetadataBuilder
	{
		Dictionary<Type, BaseEntityType> Types { get; } = new Dictionary<Type, BaseEntityType>();
		Dictionary<Type,string> EntitySysTypes { get; }
		IValueTypeResolver ValueTypeResolver { get; }

		IEnumerable<EntityAttribute> GetAttributes(MemberInfo prop)
		{
			return prop.GetCustomAttributes(false).Cast<Attribute>().Select(a => 
				new EntityAttribute(
				a.GetType().FullName,
				(from p in a.GetType().AllPublicInstanceProperties()
				where p.DeclaringType!=typeof(object) && p.DeclaringType!=typeof(Attribute)
				select (name:p.Name,value:p.GetValue(a))
				).ToDictionary(p=>p.name,p=>p.value)
				)
			);
		}

		EntityProperty BuildProperty(PropertyInfo prop)
		{
			var propType = prop.PropertyType;
			var realType = propType;
			if (propType.IsGeneric())
			{
				var ptd = propType.GetGenericTypeDefinition();
				if (ptd == typeof(IEnumerable<>) || ptd == typeof(ICollection<>))
					realType = propType.GetGenericArguments()[0];
			}
			else if (propType.IsArray)
				realType = propType.GetElementType();

			var name = prop.Name;
			var attrs = GetAttributes(prop).ToArray();

			var et = Types.Get(realType) as IType;
			if(et==null)
			{
				et = ValueTypeResolver.Resolve(name, prop.PropertyType, attrs);
				if (et == null)
					throw new ArgumentException($"找不到属性类型:{prop.PropertyType} {prop.DeclaringType}.{name}");
			}
			var mode = et is IValueType ? PropertyMode.Value : 
					realType == propType ? PropertyMode.SingleRelation : 
					PropertyMode.MultipleRelation;

			return new EntityProperty(
				prop.Name,
				mode,
				et,
				attrs
				);
		}
		public SystemTypeMetadataBuilder(IEnumerable<(string prefix, Type type)> EntityTypes, IValueTypeResolver ValueTypeResolver)
		{
			this.EntitySysTypes = EntityTypes.ToDictionary(p => p.type, p => p.prefix);
			this.ValueTypeResolver = ValueTypeResolver;
		}
		string GetEntityName(Type type)
		{
			var prefix = EntitySysTypes.Get(type);
			return prefix == null ? "Internal_"+type.FullName.Replace('.', '_') : prefix + type.Name;
		}
		BaseEntityType FindEntityType(Type Type)
		{
			if (Type == null) return null;
			return Types.Get(Type);
		}
		Type GetSysBaseType(Type Type)
		{
			var bt = Type.BaseType;
			if (bt == null || bt == typeof(object) || bt == typeof(MarshalByRefObject))
				return null;
			return bt;
		}
		public MetadataCollection Build()
		{
			var AllSysTypes =
				ADT.Tree.Build(
					EntitySysTypes.Keys,
					t => GetSysBaseType(t)
					)
				.AsEnumerable()
				.Select(n => n.Value)
				.ToArray();

			foreach(var t in AllSysTypes)
			{
				var name = GetEntityName(t);
				Types.Add(
					t,
					new EntityType(
						name,
						(EntityType)FindEntityType(GetSysBaseType(t)),
						null,
						GetAttributes(t).ToArray()
					)
				);
			}
			foreach (var t in AllSysTypes)
			{
				var et = (EntityType)Types[t];
				et.Properties = t.AllPublicInstanceProperties().Select(BuildProperty).ToArray();
			}
			return new MetadataCollection(
				EntitySysTypes.Keys
					.Select(t => Types[t])
					.OrderBy(t => t.Name)
					.Cast<EntityType>()
					.ToArray(),
				Types.ToDictionary(p=>GetEntityName(p.Key),p=>p.Value)
				);
		}
	}
}

using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;

namespace SF.Entities.AutoEntityProvider.Internals
{
	
	/*
	 * ���������͸���ʵ�����
	 * ��ʵ���������������͵�����
	 * 
	 */
	public class SystemTypeMetadataBuilder
	{
		
		Dictionary<string, EntityType> IdToEntityTypes { get; } = new Dictionary<string, EntityType>();
		Dictionary<Type, EntityType> SysTypeToEntityTypes { get; } = new Dictionary<Type, EntityType>();

		Dictionary<Type, (string ns, Type type)> EntitySysTypes { get; }
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

		EntityProperty BuildProperty(string Entity,PropertyInfo[] props)
		{
			var propTypes = props.Select(p => p.PropertyType).Distinct().ToArray();
			if (propTypes.Length > 1)
				throw new InvalidOperationException($"ʵ��{Entity}������{props[0].Name}�����Ͳ�����ȫһ��:{propTypes.Join(",")}");
			var propType = propTypes[0];
			var realType = propType;
			if (propType.IsGeneric())
			{
				var ptd = propType.GetGenericTypeDefinition();
				if (ptd == typeof(IEnumerable<>) || ptd == typeof(ICollection<>))
					realType = propType.GetGenericArguments()[0];
			}
			else if (propType.IsArray)
				realType = propType.GetElementType();

			var name = props[0].Name;
			var attrs = props.SelectMany(p=>GetAttributes(p)).ToArray();

			var et = SysTypeToEntityTypes.Get(realType) as IType;

			
			if(et==null)
			{
				//�������ʵ���࣬�������������
				et = ValueTypeResolver.Resolve(name, propType, attrs);
				if (et == null)
					throw new ArgumentException($"�Ҳ�����������:{propType} {props[0].DeclaringType}.{name}");
			}
			var mode = et is IValueType ? PropertyMode.Value : 
					realType == propType ? PropertyMode.SingleRelation : 
					PropertyMode.MultipleRelation;

			return new EntityProperty(
				name,
				mode,
				et,
				attrs
				);
		}
		public SystemTypeMetadataBuilder(IEnumerable<(string ns, Type type)> EntityTypes, IValueTypeResolver ValueTypeResolver)
		{
			this.EntitySysTypes = EntityTypes.ToDictionary(p => p.type);
			this.ValueTypeResolver = ValueTypeResolver;
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
			//��ȡ����ʵ��
			var entities =
			(from sysType in EntitySysTypes.Values
			 let eo = sysType.type.GetCustomAttribute<EntityObjectAttribute>() ?? throw new ArgumentException($"{sysType.type}δ����EntityObjectAttribute���ԣ�����ʵ�����")
			 let name = eo.Id ?? sysType.type.Name
			 let entity = sysType.ns + name
			 group (name,sysType) by entity into g
			 select (id:g.Key,n:g.First().name, types: g.Select(i => i.sysType.type).ToArray())).ToArray();

			foreach (var e in entities)
			{
				var et = new EntityType(
						e.id,
						e.n,
						null,
						e.types.SelectMany(t => GetAttributes(t)).ToArray()
					);
				IdToEntityTypes.Add(e.id,et);
				foreach (var t in e.types)
					SysTypeToEntityTypes.Add(t, et);

			}

			foreach (var e in entities)
				IdToEntityTypes[e.id].Properties =
					(from t in e.types
					 from p in t.AllPublicInstanceProperties().Select((p,idx)=>(prop:p,idx:idx))
					 group p by p.prop.Name into g
					 let idx=g.Select(i=>i.idx).Average()
					 orderby idx
					 select BuildProperty(e.id,g.Select(i=>i.prop).ToArray())
					).ToArray();

			//entities.ForEach(e =>
			//	Types.Add(
			//		e.Key,
			//		)


			//var AllSysTypes =
			//	ADT.Tree.Build(
			//		EntitySysTypes.Keys,
			//		t => GetSysBaseType(t)
			//		)
			//	.AsEnumerable()
			//	.Select(n => n.Value)
			//	.ToArray();

			//foreach(var t in AllSysTypes)
			//{
			//	var name = GetEntityName(t);
			//	Types.Add(
			//		t,
			//		new EntityType(
			//			name,
			//			(EntityType)FindEntityType(GetSysBaseType(t)),
			//			null,
			//			GetAttributes(t).ToArray()
			//		)
			//	);
			//}
			//foreach (var t in AllSysTypes)
			//{
			//	var et = (EntityType)Types[t];
			//	et.Properties = t.AllPublicInstanceProperties().Select(BuildProperty).ToArray();
			//}
			return new MetadataCollection(IdToEntityTypes);
		}
	}
}

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

	
		class EntityComparer : IEqualityComparer<(MemberInfo, EntityAttribute[], string[])>
		{
			public static EntityComparer Instance { get; } = new EntityComparer();
			public bool Equals((MemberInfo, EntityAttribute[], string[]) x, (MemberInfo, EntityAttribute[], string[]) y)
			{
				if (x.Item3.Length != y.Item3.Length)
					return false;
				return x.Item3.Zip(y.Item3, (i, j) => i == j).All(i=>i);
			}

			public int GetHashCode((MemberInfo, EntityAttribute[], string[]) obj)
			{
				return obj.Item3.Aggregate(0, (s, i) => s ^ i.GetHashCode());
			}
		}

		EntityAttribute[] MergeAttributes(IGrouping<string,(MemberInfo,EntityAttribute)> Members)
		{
			var gs= Members.GroupBy(a => a.Item1, a => a.Item2)
				.Select(g=>(g.Key,g.ToArray(),g.Select(i=>i.ToString()).OrderBy(i=>i).ToArray()))
				.Distinct(EntityComparer.Instance)
				.ToArray();
			if (gs.Length != 1)
			{
				if (gs[0].Item1 is PropertyInfo)
					throw new InvalidOperationException(
						"��ͬ���Զ�������Բ�һ�£�\n" +
						gs.Select(i => i.Item1.DeclaringType.FullName + "." + i.Item1.Name + ":" + i.Item3.Join(";")).Join("\n")
						);
				else
					throw new InvalidOperationException(
						"��ͬ���Ͷ�������Բ�һ�£�\n" +
						gs.Select(i => ((Type)i.Item1).FullName + ":" + i.Item3.Join(";")).Join("\n")
						);
			}
			return gs[0].Item2;
		}
		EntityAttribute[] MergeMemberAttributes(IEnumerable<MemberInfo> Members)
		{
			return (
			from m in Members
			 from ea in EntityAttribute.GetAttributes(m)
			 group (m, ea) by ea.Name into g
			 from a in MergeAttributes(g)
			 select a
			 ).ToArray();
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
				if (ptd == typeof(IEnumerable<>) || ptd == typeof(ICollection<>) )
					realType = propType.GetGenericArguments()[0];
			}
			else if (propType.IsArray)
				realType = propType.GetElementType();

			var name = props[0].Name;
			//if (name == "Id")
			//	throw new ArgumentException(props.Select(p => p.DeclaringType.Name+"."+p.Name+":"+ p.GetCustomAttributes().Select(a => a.GetType().Name).Join(",")).Join(";"));
			var attrs = MergeMemberAttributes(props);

			var et = SysTypeToEntityTypes.Get(realType) as IType;
			
			if(et==null)
			{
				//�������ʵ���࣬�������������
				et = ValueTypeResolver.Resolve(Entity, name, propType, attrs);
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
		public SystemTypeMetadataBuilder(
			IEnumerable<AutoEntityType> EntityTypes, 
			IValueTypeResolver ValueTypeResolver
			)
		{
			this.EntitySysTypes = EntityTypes.ToDictionary(p => p.Type,p=>(p.Namespace,p.Type));
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
			 let name = eo.Entity ?? sysType.type.Name
			 let entity = sysType.ns + name
			 group (name,sysType) by entity into g
			 select (id:g.Key,name:g.First().name,ns:g.First().sysType.ns, types: g.Select(i => i.sysType.type).ToArray())).ToArray();

			foreach (var e in entities)
			{
				var et = new EntityType(
						e.name,
						e.ns,
						null,
						MergeMemberAttributes(e.types)
					);
				IdToEntityTypes.Add(et.FullName,et);
				foreach (var t in e.types)
					SysTypeToEntityTypes.Add(t, et);

			}

			foreach (var e in entities)
			{
				var ignoreProps = new HashSet<string>(
					from t in e.types
					from p in t.AllPublicInstanceProperties()
					let ei = p.GetCustomAttribute<EntityIdentAttribute>()
					where ei?.NameField != null
					select ei.NameField
					);

				IdToEntityTypes[e.id].Properties =
					(from t in e.types
					 from p in t.AllPublicInstanceProperties()
								.Where(p=> !ignoreProps.Contains(p.Name))
								.Select((p, idx) => (prop: p, idx: idx))
					 group p by p.prop.Name into g
					 let idx = g.Select(i => i.idx).Average()
					 orderby idx
					 select BuildProperty(e.id, g.Select(i => i.prop).ToArray())
					).ToArray();
			}

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
			return new MetadataCollection(
				IdToEntityTypes,
				(from e in entities
				from t in e.types
				let ex= IdToEntityTypes[e.id]
				 select (t,ex))
				.ToDictionary(p=>p.t,p=>p.ex)
				);
		}
	}
}

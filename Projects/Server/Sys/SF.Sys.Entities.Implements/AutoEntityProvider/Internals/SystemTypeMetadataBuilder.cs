#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Reflection;
using SF.Sys.Linq;
using SF.Sys.Collections.Generic;
using SF.Sys.Annotations;
using SF.Sys.Linq.Expressions;

namespace SF.Sys.Entities.AutoEntityProvider.Internals
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

		IValueTypeResolver ValueTypeResolver { get; }
		IEntityMetadata[] EntityMetadatas { get; }


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

		EntityAttribute[] MergeAttributes(IGrouping<string,(MemberInfo,EntityAttribute)> attrs)
		{
			if(attrs.Key=="#Comment")
			{
				var values =
					(from a in attrs
					 from p in a.Item2.Values
					 let lev = a.Item1.DeclaringType.GetInheritLevel()
					 group (value: p.Value, lev: lev) by p.Key into g
					 select (g.Key, value: g.OrderByDescending(i => i.lev).Select(i => i.value).First())
				).ToDictionary(i => i.Key, i => i.value);
				return new[] { new EntityAttribute(attrs.Key, values) };
			}

			var gs= attrs.GroupBy(a => a.Item1, a => a.Item2)
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
			let name=ea.Name == typeof(DisplayAttribute).FullName ? "#Comment": ea.Name
			 group (m, ea) by name into g
			 from a in MergeAttributes(g)
			 select a
			 ).ToArray();
		}

		(Type Type, IEntityType Entity, PropertyMode Mode) GetPropType(Type Type)
		{
			var realType = Type;
			if (Type.IsGeneric())
			{
				var ptd = Type.GetGenericTypeDefinition();
				if (ptd == typeof(IEnumerable<>) || ptd == typeof(ICollection<>))
					realType = Type.GetGenericArguments()[0];
			}
			else if (Type.IsArray)
				realType = Type.GetElementType();

			var et = SysTypeToEntityTypes.Get(realType);

			return (
				et==null?Type:realType,
				et ,
				et == null ? PropertyMode.Value : 
					realType == Type ? PropertyMode.SingleRelation : 
					PropertyMode.MultipleRelation
				);
		}

		EntityProperty BuildProperty(string Entity,PropertyInfo[] props)
		{
			var propTypes = props.Select(p => GetPropType(p.PropertyType)).ToArray();
			var PropModes = propTypes.Select(pt => pt.Mode).Distinct().ToArray();
			if (PropModes.Length>1)
				throw new InvalidOperationException($"ʵ��{Entity}������{props[0].Name}��ģʽ������ȫһ��,����:{PropModes.Join(",")}");

			if (PropModes[0] == PropertyMode.Value)
			{
				var types = propTypes.Select(pt => pt.Type).Distinct().ToArray();
				if (types.Length > 1)
					throw new InvalidOperationException($"ʵ��{Entity}������{props[0].Name}�����Ͳ�����ȫһ��:{types.Join(",")}");
			}
			else
			{
				var entityTypes = propTypes.Select(pt => pt.Entity).Distinct().ToArray();
				if (entityTypes.Length > 1)
					throw new InvalidOperationException($"ʵ��{Entity}��ʵ������{props[0].Name}�����Ͳ�����ȫһ��:{entityTypes.Select(e=>e.Name).Join(",")}");
			}

			var name = props[0].Name;
			//if (name == "Id")
			//	throw new ArgumentException(props.Select(p => p.DeclaringType.Name+"."+p.Name+":"+ p.GetCustomAttributes().Select(a => a.GetType().Name).Join(",")).Join(";"));
			var attrs = MergeMemberAttributes(props);
			IType propType;
			if (PropModes[0] == PropertyMode.Value)
			{
				//�������ʵ���࣬�������������
				propType = ValueTypeResolver.Resolve(Entity, name, propTypes[0].Type, attrs);
				if (propType == null)
					throw new ArgumentException($"�Ҳ�����������:{propType} {props[0].DeclaringType}.{name}");
			}
			else
				propType = propTypes[0].Entity;
			
			return new EntityProperty(
				name,
				PropModes[0],
				propType,
				attrs
				);
		}

		public SystemTypeMetadataBuilder(
			IEntityMetadataCollection EntityMetadatas,
			IEnumerable<IEntityAutoCapability> Capabilities,
			IValueTypeResolver ValueTypeResolver
			)
		{
			this.EntityMetadatas = Capabilities
				.Where(c => c.Capability.HasFlag(AutoCapability.GenerateManager))
				.Select(c => EntityMetadatas.FindByTypeIdent(c.EntityIdent)
				?? throw new ArgumentException($"�Ҳ���ʵ���Զ�����������ʹ�õ�ʵ��{c.EntityIdent}")
				).ToArray();
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

			foreach (var e in EntityMetadatas)
			{
				var et = new EntityType(
						e.Ident,
						null,
						MergeMemberAttributes(e.EntityTypes)
					);
				IdToEntityTypes.Add(et.Name,et);
				
				foreach (var t in e.EntityTypes)
					SysTypeToEntityTypes.Add(t, et);

			}

			foreach (var e in EntityMetadatas)
			{
				var ignoreProps = new HashSet<string>(
					from t in e.EntityTypes
					from p in t.AllPublicInstanceProperties()
					let ei = p.GetCustomAttribute<EntityIdentAttribute>()
					where ei?.NameField != null
					select ei.NameField
					);

				IdToEntityTypes[e.Ident].Properties =
					(from t in e.EntityTypes
					 from p in t.AllPublicInstanceProperties()
								.Where(p=> !ignoreProps.Contains(p.Name))
								.Select((p, idx) => (prop: p, idx: idx))
					 group p by p.prop.Name into g
					 let idx = g.Select(i => i.idx).Average()
					 orderby idx
					 select BuildProperty(e.Ident, g.Select(i => i.prop).ToArray())
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
				(from e in EntityMetadatas
				let ex= IdToEntityTypes[e.Ident]
				from t in e.EntityTypes
				select (t,ex)
				).ToDictionary(p => p.t, p => p.ex)
				);
		}
	}
}

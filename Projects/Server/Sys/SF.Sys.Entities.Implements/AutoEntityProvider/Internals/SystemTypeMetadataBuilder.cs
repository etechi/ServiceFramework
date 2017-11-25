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
	 * 将所有类型根据实体分类
	 * 按实体分析所有相关类型的属性
	 * 
	 */
	public class SystemTypeMetadataBuilder
	{
		
		Dictionary<string, EntityType> IdToEntityTypes { get; } = new Dictionary<string, EntityType>();
		Dictionary<Type, EntityType> SysTypeToEntityTypes { get; } = new Dictionary<Type, EntityType>();

		IValueTypeResolver ValueTypeResolver { get; }
		IEntityMetadata[] EntityMetadatas { get; }


		class EntityComparer : IEqualityComparer<(MemberInfo member, EntityAttribute[] attrs)>
		{
			public static EntityComparer Instance { get; } = new EntityComparer();
			public bool Equals((MemberInfo member, EntityAttribute[] attrs) x, (MemberInfo member, EntityAttribute[] attrs) y)
			{
				if (x.attrs.Length != y.attrs.Length)
					return false;

				return x.attrs.Zip(
					y.attrs,
					(i, j) => i.Name == j.Name &&
								i.Values.Count == j.Values.Count &&
								i.Values.Zip(
									j.Values,
									(ivp, jvp) => ivp.Key == jvp.Key && Poco.ObjectDeepEqual(ivp.Value,jvp.Value)
								).All(v => v)
					).All(v => v);
			}

			public int GetHashCode((MemberInfo member, EntityAttribute[] attrs) obj)
			{
				return obj.attrs.Aggregate(
					0,
					(s, i) =>
						s ^
						i.Name.GetHashCode() ^
						i.Values.Aggregate(
							0,
							(si, ii) =>
								si ^
								ii.Key.GetHashCode() ^
								Poco.GetObjectDeepHashCode(ii.Value)
							)
					);
			}
		}

		EntityAttribute[] MergeAttributes(IGrouping<string,(MemberInfo member,EntityAttribute attr)> attrs)
		{
			if(attrs.Key=="#Comment")
			{
				var values =
					from a in attrs
					 from p in a.attr.Values
					 let lev = a.member.DeclaringType.GetInheritLevel()
					 group (value: p.Value, lev: lev) by p.Key into g
					 select (key: g.Key, value: g.OrderByDescending(i => i.lev).Select(i => i.value).First())
					;
				return new[] { new EntityAttribute(attrs.Key, values) };
			}

			var gs= attrs.GroupBy(a => a.member, a => a.attr)
				.Select(g => (member: g.Key,attrs:g.OrderBy(a=>a.Name).ToArray()))
				.Distinct(EntityComparer.Instance)
				.ToArray();
			if (gs.Length != 1)
			{
				if (gs[0].member is PropertyInfo)
					throw new InvalidOperationException(
						"不同属性定义的特性不一致：\n" +
						gs.Select(i => i.member.ReflectedType.FullName+"/"+i.member.DeclaringType.FullName + "." + i.member.Name + ":" + i.attrs.Join(";")).Join("\n")
						);
				else
					throw new InvalidOperationException(
						"不同类型定义的特性不一致：\n" +
						gs.Select(i => ((Type)i.Item1).FullName + ":" + i.attrs.Join(";")).Join("\n")
						);
			}
			return gs[0].attrs;
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
				throw new InvalidOperationException($"实体{Entity}的属性{props[0].Name}的模式并不完全一致,包含:{PropModes.Join(",")}");

			if (PropModes[0] == PropertyMode.Value)
			{
				var types = propTypes.Select(pt => pt.Type).Distinct().ToArray();
				if (types.Length > 1)
					throw new InvalidOperationException($"实体{Entity}的属性{props[0].Name}的类型并不完全一致:{types.Join(",")}");
			}
			else
			{
				var entityTypes = propTypes.Select(pt => pt.Entity).Distinct().ToArray();
				if (entityTypes.Length > 1)
					throw new InvalidOperationException($"实体{Entity}的实体属性{props[0].Name}的类型并不完全一致:{entityTypes.Select(e=>e.Name).Join(",")}");
			}

			var name = props[0].Name;
			//if (name == "Id")
			//	throw new ArgumentException(props.Select(p => p.DeclaringType.Name+"."+p.Name+":"+ p.GetCustomAttributes().Select(a => a.GetType().Name).Join(",")).Join(";"));
			var attrs = MergeMemberAttributes(props);
			IType propType;
			if (PropModes[0] == PropertyMode.Value)
			{
				//如果不是实体类，则查找数据类型
				propType = ValueTypeResolver.Resolve(Entity, name, propTypes[0].Type, attrs);
				if (propType == null)
					throw new ArgumentException($"找不到属性类型:{propType} {props[0].DeclaringType}.{name}");
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
				?? throw new ArgumentException($"找不到实体自动生成特性中使用的实体{c.EntityIdent}")
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

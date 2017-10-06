using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.XX
{

	public class TypeMapResult
	{
		public Type Type { get; set; }
		public IReadOnlyList<IAttribute> Attributes { get; set; }
	}
	public interface IDataModelTypeMapper
	{
		int Priority { get; }
		TypeMapResult MapType(IEntityType EntityType, IProperty Property, Type Type);
	}
	public interface IDataModelAttributeGenerator
	{
		CustomAttributeExpression Generate(IAttribute Attr);
	}
	public class BaseDataModel
	{

	}
	public interface IDataModelTypeCollection : IReadOnlyDictionary<string, Type>
	{

	}
	public class DataModelTypeCollection : Dictionary<string, Type>, IDataModelTypeCollection
	{

	}
	public class DataModelTypeBuilder
	{
		IMetadataCollection metas { get; }


		Dictionary<string, TypeReference> TypeReferences = new Dictionary<string, TypeReference>();
		//Dictionary<object, List<CustomAttributeExpression>> MemberAttributes { get; } = new Dictionary<object, List<CustomAttributeExpression>>();
		NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver { get; }
		IDataModelTypeMapper[] DataModelTypeMappers { get; }
		IDynamicTypeBuilder DynamicTypeBuilder { get; }
		public DataModelTypeBuilder(
			IMetadataCollection metas,
			NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver,
			IEnumerable<IDataModelTypeMapper> DataModelTypeMappers,
			IDynamicTypeBuilder DynamicTypeBuilder
			)
		{
			this.DynamicTypeBuilder = DynamicTypeBuilder;
			this.DataModelTypeMappers = DataModelTypeMappers.OrderBy(p => p.Priority).ToArray();
			this.metas = metas;
			this.DataModelAttributeGeneratorResolver = DataModelAttributeGeneratorResolver;
		}
		//void AddAttribute(object member, CustomAttributeExpression attr)
		//{
		//	if (!MemberAttributes.TryGetValue(member, out var attrs))
		//		MemberAttributes.Add(member, attrs = new List<CustomAttributeExpression>());
		//	attrs.Add(attr);
		//}
		void EnsureSingleFieldIndex(PropertyExpression propExpr, IEntityType type, IProperty prop)
		{
			//已经有索引
			int idx;
			if (propExpr.CustomAttributes.Any(a =>
					a.Constructor.ReflectedType == typeof(IndexAttribute) &&
					(
					(a.Arguments.Count() == 0 && a.InitProperties.Count() == 0)//单一索引
					|| (-1 != (idx = a.InitProperties.IndexOf(p => p.Name == "Order")) && (1 == Convert.ToInt32(a.InitPropertyValues.At(idx)))) //多列索引第一项1
					|| (a.Arguments.Count() == 2 && (1 == Convert.ToInt32(a.Arguments.At(1)))) //多列索引第一项2
					)
				))
				return;

			//当前字段是唯一主键
			if (prop.Attributes.Any(a =>
					a.Name == typeof(KeyAttribute).FullName &&
					type.Properties.Count(p => p.Attributes.Any(aa => aa.Name == typeof(KeyAttribute).FullName)) == 1
				))
				return;
			propExpr.CustomAttributes.Add(
				new CustomAttributeExpression(typeof(IndexAttribute).GetConstructor(Array.Empty<Type>()))
				);
		}

		PropertyExpression BuildProperty(string name, TypeReference type, IEnumerable<CustomAttributeExpression> attrs = null)
		{
			var pe = new PropertyExpression(name, type, PropertyAttributes.None);
			if (attrs != null)
				pe.CustomAttributes.AddRange(attrs);
			return pe;
		}

		PropertyExpression DefineProperty(
			IEntityType entityType,
			string Name,
			TypeReference type,
			IEnumerable<IAttribute> Attributes
			)
		{
			return BuildProperty(
				Name,
				type,
				Attributes?.Select(a =>
				{
					var g = DataModelAttributeGeneratorResolver(a.Name);
					if (g == null)
						throw new ArgumentException($"不支持生成特性{a.Name}的数据类特性，实体类型:{entityType.Name} 属性{Name}");
					return g.Generate(a);
				})?.Where(a => a != null)
				);
		}
		PropertyExpression DefineProperty(IEntityType entityType, IProperty prop, TypeReference type, IReadOnlyList<IAttribute> attrs) =>
			DefineProperty(entityType, prop.Name, type, attrs);

		void EnsureForginKeyField(TypeExpression typeExpr, IEntityType curEntity, IEntityType foreignEntity, string name)
		{
			var fepk = foreignEntity.Properties.Where(p => p.Attributes.Any(a => a.Name == typeof(KeyAttribute).FullName)).ToArray();
			if (fepk.Length == 0)
				throw new InvalidOperationException($"定义实体{curEntity.FullName}的外键属性{name}时,外键实体上{foreignEntity.FullName}找不到主键");
			else if (fepk.Length > 1)
				throw new InvalidOperationException($"定义实体{curEntity.FullName}的外键属性{name}时,不支持外键实体{foreignEntity.FullName}上有多个主键{fepk.Select(p => p.Name).Join(",")}");
			var propType = fepk[0].Type;
			if (!(propType is IValueType))
				throw new InvalidOperationException($"实体{foreignEntity.FullName}主键{fepk[0].Name}不是数据类型，而是类型{fepk[0].Type.Name}");
			var propSysType = ((IValueType)propType).SysType;

			PropertyExpression pb;
			var prop = curEntity.Properties.FirstOrDefault(p => p.Name == name);
			if (prop == null)
				pb = DefineProperty(curEntity, name, new SystemTypeReference(propSysType), null);
			else
			{
				if (prop.Mode != PropertyMode.Value)
					throw new InvalidOperationException($"实体{curEntity.FullName}的属性{prop.Name}类型为{prop.Type.Name}的关系字段，不是类型为{propSysType}的{foreignEntity.FullName}的外部关系外键");
				else if (((IValueType)prop.Type).SysType != propSysType)
					throw new InvalidOperationException($"实体{curEntity.FullName}的外键{fepk[0].Name}类型为{((IValueType)prop.Type).SysType},和外键实体{foreignEntity.FullName}主键{fepk[0].Name}的类型{propSysType}不一致");
				pb = typeExpr.Properties.Single(p => p.Name == name);
			}

			//确保外键字段有索引
			if (pb != null)
				EnsureSingleFieldIndex(pb, curEntity, prop);
		}
		void EnsureForginRelationField(TypeExpression typeExpr, IEntityType curEntity, IEntityType foreignEntity, string name)
		{
			var prop = curEntity.Properties.SingleOrDefault(p => p.Name == name);
			if (prop == null)
				typeExpr.Properties.Add(DefineProperty(curEntity, name, TypeReferences[foreignEntity.FullName], null));
			else if (prop.Mode != PropertyMode.SingleRelation)
				throw new InvalidOperationException($"实体{curEntity.FullName}的属性{prop.Name}类型为{prop.Type.Name}的数据字段，不是{foreignEntity.FullName}外部关系");
			else if (prop.Type != foreignEntity)
				throw new InvalidOperationException($"实体{curEntity.FullName}的属性{prop.Name}的关系为{prop.Type.Name}，不是{foreignEntity.FullName}外部关系");

			var idFieldName = prop?.Attributes
				.FirstOrDefault(a => a.Name == typeof(ForeignKeyAttribute).FullName)
				.Values.Get("Name") as string ??
				(name + "Id");

			EnsureForginKeyField(typeExpr, curEntity, foreignEntity, idFieldName);
		}

		TypeMapResult MapSysPropType(IEntityType type, IProperty prop, Type sysType)
		{
			return DataModelTypeMappers
				.Select(p => p.MapType(type, prop, sysType))
				.FirstOrDefault(t => t != null) ??
				new TypeMapResult
				{
					Type = sysType,
					Attributes = prop.Attributes
				};
		}
		void BuildValueProperty(TypeExpression typeExpr, IEntityType type, IProperty prop)
		{
			var MapTypeResult = MapSysPropType(type, prop, ((IValueType)prop.Type).SysType);
			var sysType = MapTypeResult.Type;

			var pb = DefineProperty(
				type,
				prop,
				new SystemTypeReference(sysType),
				MapTypeResult.Attributes
				);
			typeExpr.Properties.Add(pb);
			//处理自动生成主键字段
			if (sysType == typeof(long) &&
				(prop.Attributes?.Any(a => a.Name == typeof(KeyAttribute).FullName) ?? false) &&
				!(prop.Attributes?.Any(a => a.Name == typeof(DatabaseGeneratedAttribute).FullName) ?? false) &&
				!type.Properties.Any(p => p != prop && (p.Attributes?.Any(a => a.Name == typeof(KeyAttribute).FullName) ?? false))
				)
			{
				pb.CustomAttributes.Add(
					new CustomAttributeExpression(
						typeof(DatabaseGeneratedAttribute).GetConstructor(new[] { typeof(DatabaseGeneratedOption) }),
						new object[] { DatabaseGeneratedOption.None }
						)
					);
			}

			//检查当前如果有使用字段作为外键的字段，则当前字段需要增加索引
			var foreignProp = type.Properties.Where(p =>
				   p.Mode == PropertyMode.SingleRelation &&
				   (p.Name + "Id" == prop.Name ||
				   p.Attributes.Any(a => a.Name == typeof(ForeignKeyAttribute).FullName && a?.Values.Get("Name") as string == prop.Name)
				   )).SingleOrDefault();
			if (foreignProp != null)
				EnsureForginKeyField(typeExpr, type, (IEntityType)foreignProp.Type, prop.Name);

			//如果是实体标识，需要创建索引
			if (prop.Attributes.Any(a => a.Name == typeof(EntityIdentAttribute).FullName))
				EnsureSingleFieldIndex(pb, type, prop);
		}
		void BuildSingleRelationProperty(TypeExpression typeExpr, IEntityType type, IProperty prop)
		{
			var pd = DefineProperty(
				type,
				prop,
				TypeReferences[((IEntityType)prop.Type).FullName],
				prop.Attributes
				);
			typeExpr.Properties.Add(pd);
			//确保外部关系字段
			EnsureForginRelationField(typeExpr, type, (IEntityType)prop.Type, prop.Name);

		}
		void BuildMultipleRelationProperty(TypeExpression typeExpr, IEntityType type, IProperty prop)
		{
			var pd = DefineProperty(
				type,
				prop,
				new GenericTypeReference(
					new SystemTypeReference(typeof(ICollection<>)),
					TypeReferences[((IEntityType)prop.Type).FullName]
					),
				prop.Attributes
				);
			typeExpr.Properties.Add(pd);
			//确保外键字段
			var foreignType = (IEntityType)prop.Type;
			var ForeignRelationPropName = pd.CustomAttributes
				.FirstOrDefault(a => a.Constructor.ReflectedType == typeof(InversePropertyAttribute))
				?.Arguments
				.First() as string ?? type.Name;
			EnsureForginRelationField(typeExpr, foreignType, type, ForeignRelationPropName);

		}

		TypeExpression BuildType(TypeExpression typeExpr, IEntityType type, string TablePrefix)
		{
			var keys = type.Properties.Where(p => p.Attributes.Any(a => a.Name == typeof(KeyAttribute).FullName)).ToArray();
			if (keys.Length == 0)
				throw new ArgumentException($"实体{type.FullName}未定义主键");
			else
			{
				var nks = keys.Where(k => !(k.Type is IValueType)).ToArray();
				if (nks.Length > 0)
					throw new ArgumentException($"实体{type.FullName}的主键字段不为原子类型{nks.Select(n => n.Name + ":" + n.Type.Name).Join(",")}");
				if (keys.Length == 1)
				{
					typeExpr.ImplementInterfaces.Add(
						new SystemTypeReference(
							typeof(IEntityWithId<>).MakeGenericType(((IValueType)keys[0].Type).SysType)
						)
					);
				}
			}

			foreach (var a in type.Attributes.Where(a => a.Name != typeof(TableAttribute).FullName))
			{
				var g = DataModelAttributeGeneratorResolver(a.Name);
				if (g == null)
					throw new ArgumentException($"不支持生成特性{a.Name}的数据类特性，实体类型:{typeExpr.Name}");
				var aa = g.Generate(a);
				if (aa != null)
					typeExpr.CustomAttributes.Add(aa);

			}
			var tableName = type.Name;
			var tableAttr = type.Attributes.FirstOrDefault(a => a.Name == typeof(TableAttribute).FullName);
			if (tableAttr != null)
				tableName = tableAttr.Values?.Get("Name") as string ?? tableName;

			typeExpr.CustomAttributes.Add(new CustomAttributeExpression(
				typeof(TableAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { (TablePrefix ?? "") + (type.Namespace ?? "") + tableName }
				));

			foreach (var prop in type.Properties)
				switch (prop.Mode)
				{
					case PropertyMode.Value:
						BuildValueProperty(typeExpr, type, prop);
						break;
					case PropertyMode.MultipleRelation:
						BuildValueProperty(typeExpr, type, prop);
						break;
					case PropertyMode.SingleRelation:
						BuildValueProperty(typeExpr, type, prop);
						break;
					default:
						throw new NotSupportedException();
				}
			return typeExpr;
		}
		static volatile int TypeIdSeed = 1;
		static int NextTypeId()
		{
			return System.Threading.Interlocked.Increment(ref TypeIdSeed);
		}

		public IDataModelTypeCollection Build(string Prefix)
		{
			//throw new ArgumentException(metas.ToString());
			foreach (var et in metas.EntityTypes)
			{
				TypeReferences.Add(
					et.Key,
					new TypeExpressionReference(new TypeExpression(
						et.Key + "_" + NextTypeId(),
						new SystemTypeReference(typeof(BaseDataModel)),
						TypeAttributes.Public
						)
					)
					);
			}

			var re = new DataModelTypeCollection();
			var exprs = (from entityType in metas.EntityTypes.Values
						 let typeExpr = BuildType(
							 (TypeReferences[entityType.FullName] as TypeExpressionReference).Type,
							 entityType,
							 Prefix
							 )
						 select (entityType.FullName, typeExpr)
						).ToArray();

			var types = DynamicTypeBuilder.Build(exprs.Select(e => e.typeExpr));

			exprs
				.Zip(types, (x, type) => (x.FullName, type))
				.ForEach(p => re.Add(p.FullName, p.type));
			return re;
		}
	}
}

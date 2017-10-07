using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Entities.AutoEntityProvider.Internals
{
	public class EntityAttribute : IAttribute
	{
		public string Name { get; }
		public IReadOnlyDictionary<string, object> Values { get; }
		public EntityAttribute(string Name, IReadOnlyDictionary<string, object> Values)
		{
			this.Name = Name;
			this.Values = Values;
		}
		public override string ToString()
		{
			return Name + "(" + Values?.Select(p => p.Key + "=" + p.Value).Join(";") + ")";
		}
		static object GetAttributePropertyValue(Attribute attr,PropertyInfo prop)
		{
			try
			{
				return prop.GetValue(attr);
			}
			catch
			{
				return null;
			}
		}
		public static IEnumerable<EntityAttribute> GetAttributes(MemberInfo member)
		{
			return member.GetCustomAttributes().Cast<Attribute>().Select(a =>
				new EntityAttribute(
				a.GetType().FullName,
				(from p in a.GetType().AllPublicInstanceProperties()
				 where p.DeclaringType != typeof(object) && p.DeclaringType != typeof(Attribute)
				 let value = GetAttributePropertyValue(a, p)
				 where value != null
				 select (name: p.Name, value:  value)
				).ToDictionary(p => p.name, p => p.value)
				)
			);
		}
	}
	public abstract class EntityMetaItem:IMetaItem
	{
		public string Name { get; }
		public IReadOnlyList<EntityAttribute> Attributes { get; }
		IReadOnlyList<IAttribute> IMetaItem.Attributes => Attributes;
		public EntityMetaItem(string Name, IReadOnlyList<EntityAttribute> Attributes)
		{
			this.Name = Name;
			this.Attributes = Attributes;
		}

		
	}
	public class EntityProperty : EntityMetaItem,IProperty
	{
		public IType Type { get; }
		public PropertyMode Mode { get; }

		public EntityProperty(string name, PropertyMode Mode , IType Type, IReadOnlyList<EntityAttribute> Attributes):
			base(name,Attributes)
		{
			this.Type = Type;
			this.Mode = Mode; 
		}
		public override string ToString()
		{
			return Name + "/" + Type.Name + "/" + Mode + " " + Attributes?.Join(" ");
		}
	}
	public class BaseEntityType : EntityMetaItem,IType
	{
		public BaseEntityType(string Name, IReadOnlyList<EntityAttribute> Attributes) : 
			base(Name, Attributes)
		{
		}
	}
	public class EntityValueType : BaseEntityType, IValueType
	{
		public Type SysType { get; }
		public EntityValueType(string Name, Type SysType,IReadOnlyList<EntityAttribute> Attributes) : base(Name, Attributes)
		{
			this.SysType = SysType;
		}
		public override string ToString()
		{
			return Name + "/" + SysType.FullName+ " " + Attributes?.Join(" ");
		}
		public IValueTypeProvider Provider => throw new NotImplementedException();
	}
	public class EntityType : BaseEntityType, IEntityType
	{
		public EntityType(string Name,IReadOnlyList<EntityProperty> Properties, EntityAttribute[] Attributes) : base(Name, Attributes)
		{
			this.Properties = Properties;
		}
		public IReadOnlyList<EntityProperty> Properties { get; set; }

		IReadOnlyList<IProperty> IEntityType.Properties => Properties;

		public override string ToString()
		{
			return Name + "\n" + Properties.Select(p => "\t" + p.ToString() ).Join("\n") + "\n";
		}
	}
	public class MetadataCollection : IMetadataCollection
	{

		public IReadOnlyDictionary<string, IEntityType> EntityTypes { get; }
		public IReadOnlyDictionary<Type, IEntityType> EntityTypesByType { get; }
		public override string ToString()
		{
			return EntityTypesByType
				.GroupBy(t => t.Value,t=>t.Key)
				.Select(g => g.Key.ToString() + "From :" + g.Select(i=>i.ToString()).Join(";") + "\n")
				.Join("\n");
		}
		public MetadataCollection( IReadOnlyDictionary<string, EntityType> EntityTypes,IReadOnlyDictionary<Type,EntityType> TypedEntities)
		{
			this.EntityTypes = EntityTypes.ToDictionary(p=>p.Key,p=>p.Value as IEntityType);
			this.EntityTypesByType = TypedEntities.ToDictionary(p => p.Key, p => p.Value as IEntityType);
		}
	}
}

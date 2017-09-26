using System;
using System.Collections.Generic;
using System.Linq;

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
	}
	public abstract class EntityMetaItem:IMetaItem
	{
		public string Name { get; }
		IReadOnlyList<EntityAttribute> Attributes { get; }
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

		public IValueTypeProvider Provider => throw new NotImplementedException();
	}
	public class EntityType : BaseEntityType, IEntityType
	{
		public string FullName { get; }
		public EntityType(string Name, string FullName ,IReadOnlyList<EntityProperty> Properties, EntityAttribute[] Attributes) : base(Name, Attributes)
		{
			this.FullName = FullName;
			this.Properties = Properties;
		}
		public IReadOnlyList<EntityProperty> Properties { get; set; }

		IReadOnlyList<IProperty> IEntityType.Properties => Properties;
	}
	public class MetadataCollection : IMetadataCollection
	{
		IReadOnlyDictionary<string, EntityType> EntityTypes { get; }

		IReadOnlyDictionary<string, IEntityType> InterfaceEntityTypes { get; }

		IReadOnlyDictionary<string, IEntityType> IMetadataCollection.EntityTypes => InterfaceEntityTypes;
		public MetadataCollection( IReadOnlyDictionary<string, EntityType> EntityTypes)
		{
			this.EntityTypes = EntityTypes;
			InterfaceEntityTypes = EntityTypes.ToDictionary(p=>p.Key,p=>p.Value as IEntityType);
		}
	}
}

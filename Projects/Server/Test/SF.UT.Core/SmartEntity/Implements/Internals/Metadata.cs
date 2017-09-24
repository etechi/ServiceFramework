using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;
using System.Linq;

namespace SF.Entities.Smart.Internals
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
	public class EntityPropertyType : BaseEntityType, IValueType
	{
		public EntityPropertyType(string Name, IReadOnlyList<EntityAttribute> Attributes) : base(Name, Attributes)
		{
		}
	}
	public class EntityType : BaseEntityType, IEntityType
	{
		public EntityType(string Name, EntityType BaseType, IReadOnlyList<EntityProperty> Properties, EntityAttribute[] Attributes) : base(Name, Attributes)
		{
			this.BaseType = BaseType;
			this.Properties = Properties;
		}
		public EntityType BaseType { get; }
		public IReadOnlyList<EntityProperty> Properties { get; set; }
		IEntityType IEntityType.BaseType => BaseType;

		IReadOnlyList<IProperty> IEntityType.Properties => Properties;
	}
	public class MetadataCollection : IMetadataCollection
	{
		IReadOnlyList<EntityType> Entities { get; }
		IReadOnlyDictionary<string, BaseEntityType> Types { get; }

		IReadOnlyList<IEntityType> InterfaceEntities { get; }
		IReadOnlyDictionary<string, IType> InterfaceTypes { get; }

		IReadOnlyList<IEntityType> IMetadataCollection.Entities => InterfaceEntities;
		IReadOnlyDictionary<string, IType> IMetadataCollection.Types => InterfaceTypes;
		public MetadataCollection(IReadOnlyList<EntityType> Entities, IReadOnlyDictionary<string, BaseEntityType> Types)
		{
			this.Entities = Entities;
			this.Types = Types;
			InterfaceEntities = Entities.Cast<IEntityType>().ToArray();
			InterfaceTypes = Types.ToDictionary(p => p.Key, p => (IType)p.Value);
		}
	}
}

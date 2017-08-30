using SF.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Metadata
{
	public interface IMetadataTypeCollection
	{
		Models.Type FindType(string Name);
		void AddType(Models.Type type);
		IEnumerable<Models.Type> GetTypes();
	}
	public interface IMetadataAttributeValuesProvider
	{
		object GetValues(System.Attribute Attribute, object AttrSource);
	}
	public interface IMetadataAttributeValuesProvider<T> : IMetadataAttributeValuesProvider
	{ }
	public interface IMetadataBuilder
	{
		IMetadataTypeCollection TypeCollection { get; }
		IJsonSerializer JsonSerializer { get; }
		string FormatTypeName(Type type);
		Models.Type TryGenerateAndAddType(Type type);
		Models.Type GenerateAndAddType(Type type);
		Models.Property GenerateTypeProperty(System.Reflection.PropertyInfo prop, object DefaultValueObject);
		Models.Property[] GenerateTypeProperties(Type type);
		T LoadAttributes<T>(T item, IEnumerable<Attribute> attrs, object attrSource, Predicate<Attribute> predicate = null) where T : Models.Entity;
	}
}

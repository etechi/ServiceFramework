

namespace SF.Metadata.Models
{
	public class AttributeValue
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}
	public class Attribute
	{
		public string Type { get; set; }
		public string Values { get; set; }
	}
	public class Entity
	{
		public Attribute[] Attributes { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Group { get; set; }
		public string Prompt { get; set; }
		public string ShortName{get;set;}
	}
	public class Type : Entity
	{
		public string[] BaseTypes { get; set; }
		public string ElementType { get; set; }
		public bool IsEnumType { get; set; }
		public bool IsArrayType { get; set; }
		public bool IsDictType { get; set; }
		public bool IsInterface { get; set; }
		public Property[] Properties { get; set; }
	}
	public class TypedEntity : Entity
	{
		public string Type { get; set; }

	}
	public class Property : TypedEntity
	{
		public bool Optional { get; set; }
        public string DefaultValue { get; set; }
	}
	
	public class Method : TypedEntity
	{
		public Parameter[] Parameters { get; set; }
	}

	public class Parameter : TypedEntity
	{
		public bool Optional { get; set; }
        public string DefaultValue { get; set; }
	}
	public class Library
	{
		public Type[] Types { get; set; }
	}
}

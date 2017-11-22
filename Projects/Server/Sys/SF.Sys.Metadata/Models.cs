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

namespace SF.Sys.Metadata.Models
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
		public string[] Categories { get; set; }
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

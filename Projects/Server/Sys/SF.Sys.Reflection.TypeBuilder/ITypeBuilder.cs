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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;

/// <summary>
/// asdasdasd111111111111111111111
/// </summary>
namespace System.Linq.TypeExpressions
{

	/// <summary>
	/// asdas121212
	/// </summary>
	public class CustomAttributeExpression
	{
		/// <summary>
		/// asdasdad
		/// </summary>
		/// <title>asdasdas</title>
		/// <title>asdasdasd</title>
		/// <remarks>asdasd</remarks>
		public ConstructorInfo Constructor { get; }
		/// <summary>
		/// asdasda
		/// </summary>
		public IEnumerable<object> Arguments { get; }
		/// <summary>
		/// asdasdasd
		/// </summary>
		public IEnumerable<PropertyInfo> InitProperties { get; }
		/// <summary>
		/// asdasdasdasdasd
		/// </summary>
		public IEnumerable<object> InitPropertyValues { get; }
		/// <summary>
		/// asdasdadad
		/// </summary>
		/// <param name="Constructor">
		///		<summary>asdasd</summary>
		///		<remarks>asdfasdfasdfasdfas</remarks>
		/// </param>
		/// <param name="Arguments">asdasd</param>
		/// <param name="InitProperties"></param>
		/// <param name="InitPropertyValues"></param>
		public CustomAttributeExpression(
			ConstructorInfo Constructor,
			IEnumerable<object> Arguments = null,
			PropertyInfo[] InitProperties = null,
			IEnumerable<object> InitPropertyValues = null
		)
		{
			this.Constructor = Constructor;
			this.Arguments = Arguments ?? Array.Empty<object>();
			this.InitProperties = InitProperties ?? Array.Empty<PropertyInfo>();
			this.InitPropertyValues = InitPropertyValues ?? Array.Empty<object>();
		}
	}
	/// <summary>
	/// asdasdasd
	/// </summary>
	public abstract class MemberExpression
	{
		public List<CustomAttributeExpression> CustomAttributes { get; } = new List<CustomAttributeExpression>();
		public MemberExpression(string Name)
		{
			this.Name = Name;
		}
		public string Name { get; }
	}
	public abstract class TypeReference
	{
	}
	public class GenericTypeReference: TypeReference
	{
		public TypeReference GenericTypeDefine { get; }
		public IEnumerable<TypeReference> GenericTypeArguments { get; }
		public GenericTypeReference(TypeReference GenericTypeDefine, params TypeReference[] GenericTypeArguments)
		{
			this.GenericTypeArguments = GenericTypeArguments;
			this.GenericTypeDefine = GenericTypeDefine;
		}
	}
	public class SystemTypeReference : TypeReference
	{
		public Type Type { get;  }
		public SystemTypeReference(Type Type)
		{
			this.Type = Type;
		}
	}
	public class TypeExpressionReference : TypeReference
	{
		public TypeExpression Type{ get; }
		public TypeExpressionReference(TypeExpression Type)
		{
			this.Type = Type;
		}
	}
	public class PropertyExpression : MemberExpression
	{
		public TypeReference PropertyType { get; }
		public PropertyAttributes Attributes { get; }
		public PropertyExpression(
			string Name, 
			TypeReference PropertyType , 
			PropertyAttributes Attributes ,
			IEnumerable<CustomAttributeExpression> CustomAttributes=null
			) : base(Name)
		{
			this.Attributes = Attributes;
			this.PropertyType = PropertyType;
			if (CustomAttributes != null)
				this.CustomAttributes.AddRange(CustomAttributes);
		}
	}
	public class TypeExpression : MemberExpression
	{
		public TypeReference BaseType { get; }
		public TypeAttributes Attributes { get; }
		public List<TypeReference> ImplementInterfaces { get; } = new List<TypeReference>();
		
		public List<PropertyExpression> Properties { get; } = new List<PropertyExpression>();
		public TypeExpression(
			string Name,
			TypeReference BaseType,
			TypeAttributes Attributes
			) : base(Name)
		{
			this.Attributes = Attributes;
			this.BaseType = BaseType;
		}
	}
	
	public interface IDynamicTypeBuilder
	{
		Type[] Build(IEnumerable<TypeExpression> Expression);
	}
}

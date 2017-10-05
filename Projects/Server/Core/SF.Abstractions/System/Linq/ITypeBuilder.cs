using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;

namespace System.Linq.TypeExpressions
{
	public class CustomAttributeExpression
	{
		public ConstructorInfo Constructor { get; }
		public IEnumerable<object> Arguments { get; }
		public IEnumerable<PropertyInfo> InitProperties { get; }
		public IEnumerable<object> InitPropertyValues { get; }
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

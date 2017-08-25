using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities.AttributeValidators
{
	public class AttributeValidateError
	{
		public string Path { get; set; }
		public string Message { get; set; }
	}
	public class AttributeValidateException : PublicArgumentException
	{
		public AttributeValidateError[] Errors { get; }
		public AttributeValidateException(AttributeValidateError[] Errors,string Message) : base(Message)
		{
			this.Errors = Errors;
		}
		public AttributeValidateException(AttributeValidateError[] Errors,string Message,Exception exception) : base(Message,exception)
		{
			this.Errors = Errors;
		}
	}
	public interface IObjectAttributeValidator
	{
		void Validate(object Value,string Name=null);
	}
	public interface IValueValidator
	{
		Type ValueType { get; }
		Type AttrType { get; }
	}
	public interface IValueValidator<TValue,TAttr> : IValueValidator
		where TAttr:Attribute
	{
		string Validate(TValue value, TAttr Attr);
	}

}

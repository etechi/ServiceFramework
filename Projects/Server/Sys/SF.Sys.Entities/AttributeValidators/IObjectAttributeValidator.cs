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

namespace SF.Sys.Entities.AttributeValidators
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

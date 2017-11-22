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

using SF.Sys.Collections.Generic;
using SF.Sys.Comments;
using SF.Sys.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Entities.AttributeValidators
{
	
	public class ObjectAttributeValidator : IObjectAttributeValidator
	{
		System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, ResultBuilder, object>> Validators { get; } = new System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, ResultBuilder, object>>();
		Dictionary<Type, IValueValidator> ValueValidatorDict { get; }
		class ResultBuilder
		{
			public List<AttributeValidateError> Errors;
			void TryAddError(string Error,Type Type,PropertyInfo Prop,string PropPath)
			{
				if (Error == null)
					return;
				if (Errors == null) Errors = new List<AttributeValidateError>();
				Errors.Add(
					new AttributeValidateError
					{
						Message= string.Format(Error, Prop.Comment()?.Title ?? Prop.Name),
						Path=PropPath
					}					
					);
			}
		}
		class ValidatorBuilder : ObjectTransform<ResultBuilder>
		{
			public Dictionary<Type, IValueValidator> ValueValidatorDict { get; set; }
			protected override string GetPropPathName(PropertyInfo Prop)
			{
				return Prop.Comment()?.Title ?? Prop.Name;
			}
			protected override Expression EvalPropExpression(Expression src, Expression PropPathExpr, string PropPath, PropertyInfo Prop)
			{
				var re=base.EvalPropExpression(src, PropPathExpr, PropPath, Prop);
				var exprs = new List<Expression>(
					from attr in Prop.GetCustomAttributes()
					let vv = ValueValidatorDict.Get(attr.GetType())
					where vv != null
					let validatorType = typeof(IValueValidator<,>).MakeGenericType(vv.ValueType, attr.GetType())

					select ArgContext.CallMethod(
						"TryAddError",
						Expression.Call(
							Expression.Convert(
								Expression.Constant(vv),
								validatorType
								),
							validatorType.GetMethod("Validate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod),
							Expression.Convert(Expression.Property(src, Prop), vv.ValueType),
							Expression.Convert(Expression.Constant(attr), attr.GetType())
						),
						Expression.Constant(src.Type),
						Expression.Constant(Prop),
						Expression.Constant(PropPath)
						)
					);
				if (re != null)
					exprs.Add(re);
				return exprs.Count==0?null:exprs.Count==1?exprs[0]:Expression.Block(exprs);
			}
		}
		

		Func<object,ResultBuilder,object> BuildValidator(Type Type)
		{

			var builder = new ValidatorBuilder();
			builder.ValueValidatorDict = ValueValidatorDict;
			return builder.Build(Type);
		}
		Func<Type, Func<object, ResultBuilder, object>> BuildValidatorFunc { get; }

		Func<object, ResultBuilder, object> GetValidator(Type Type)=>
			Validators.GetOrAdd(Type, BuildValidatorFunc);

		public ObjectAttributeValidator(IEnumerable<IValueValidator> ValueValidators)
		{
			this.ValueValidatorDict = ValueValidators.ToDictionary(vv => vv.AttrType);
			this.BuildValidatorFunc = BuildValidator;
		}

		public void Validate(object Value,string Name=null)
		{
			if (Value == null)
				throw new PublicArgumentException("需要提供" + (Name??"参数"));
			var v = GetValidator(Value.GetType());
			var rb = new ResultBuilder();
			v(Value, rb);
			if (rb.Errors != null)
				throw new AttributeValidateException(rb.Errors.ToArray(), (Name ?? "参数") + "错误");
		}
	}
}

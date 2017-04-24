using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using SF.Data.Entity;
using SF.Data.AttributeValidators;
namespace SF.Core.DI
{
	public static class ValidatorDICollectionExtension
	{
		public static IDIServiceCollection UseDataAttributeValidator(this IDIServiceCollection sc)
		{
			sc.Normal().AddSingleton<IObjectAttributeValidator, ObjectAttributeValidator>();
			sc.Normal().AddSingleton<IValueValidator, MaxLengthValidator> ();
			sc.Normal().AddSingleton<IValueValidator, MinLengthValidator>();
			sc.Normal().AddSingleton<IValueValidator, RangeValidator>();
			sc.Normal().AddSingleton<IValueValidator, RequiredValidator>();

			return sc;
		}
	}
}

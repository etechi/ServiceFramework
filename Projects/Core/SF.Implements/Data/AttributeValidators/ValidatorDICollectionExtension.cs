using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using SF.Data.Entity;
using SF.Data.AttributeValidators;
namespace SF.Core.ServiceManagement
{
	public static class ValidatorDICollectionExtension
	{
		public static IServiceCollection UseDataAttributeValidator(this IServiceCollection sc)
		{
			sc.AddSingleton<IObjectAttributeValidator, ObjectAttributeValidator>();
			sc.AddSingleton<IValueValidator, MaxLengthValidator> ();
			sc.AddSingleton<IValueValidator, MinLengthValidator>();
			sc.AddSingleton<IValueValidator, RangeValidator>();
			sc.AddSingleton<IValueValidator, RequiredValidator>();

			return sc;
		}
	}
}

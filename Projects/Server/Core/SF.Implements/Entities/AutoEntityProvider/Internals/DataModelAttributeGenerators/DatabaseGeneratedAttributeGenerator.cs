using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class DatabaseGeneratedAttributeGenerator : IDataModelAttributeGenerator
	{
		public SystemAttributeBuilder Generate(IAttribute Attr)
		{
			DatabaseGeneratedOption o;
			var dgo = Attr.Values.Get("DatabaseGeneratedOption");
			if (dgo is string s)
				o = (DatabaseGeneratedOption)Enum.Parse(typeof(DatabaseGeneratedOption), s);
			else
				o = (DatabaseGeneratedOption)Convert.ChangeType(dgo, typeof(DatabaseGeneratedOption));
			return new SystemAttributeBuilder(
				typeof(DatabaseGeneratedAttribute).GetConstructor(new[] { typeof(DatabaseGeneratedOption) }),
				new object[] {o}
				);
		}
	}

}

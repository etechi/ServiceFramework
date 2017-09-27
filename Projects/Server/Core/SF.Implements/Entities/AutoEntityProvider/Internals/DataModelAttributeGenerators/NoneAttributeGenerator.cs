using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class NoneAttributeGenerator : IDataModelAttributeGenerator
	{
		public SystemAttributeBuilder Generate(IAttribute Attr)
		{

			return null;
		}
	}

}

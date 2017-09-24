using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;

namespace SF.Entities.Smart
{
	public interface IValueTypeResolver
	{
		IValueType Resolve(string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes);
	}
}

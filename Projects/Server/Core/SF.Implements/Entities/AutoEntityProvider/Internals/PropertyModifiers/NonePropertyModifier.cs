using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider.Internals.EntityModifiers
{
	public class NonePropertyModifier : INoneEntityPropertyModifier
	{
		public int Priority { get; }
		public NonePropertyModifier(int Priority)
		{
			this.Priority = Priority;
		}
	}
}

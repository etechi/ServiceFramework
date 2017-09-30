using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SF.Entities
{
	public static class ObjectKey 
	{
		public static ObjectKey<T> From<T>(T Id)
			where T : IEquatable<T>
		{
			return new ObjectKey<T> { Id = Id };
		}
	}
}

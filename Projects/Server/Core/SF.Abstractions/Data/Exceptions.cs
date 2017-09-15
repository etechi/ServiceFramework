using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public class DbException : Exception
	{
		public DbException(string message, Exception ie) : base(message, ie) { }
	}
	public class DbValidateException : DbException
	{
		public DbValidateException(string message, Exception ie) : base(message, ie) { }
	}
	public class DbDuplicatedKeyException : DbException
	{
		public DbDuplicatedKeyException(string message, Exception ie) : base(message, ie) { }
	}
	public class DbUpdateException : DbException
	{
		public DbUpdateException(string message, Exception ie) : base(message, ie) { }
	}
	public class DbUpdateConcurrencyException : DbException
	{
		public DbUpdateConcurrencyException(string message, Exception ie) : base(message, ie) { }
	}
}

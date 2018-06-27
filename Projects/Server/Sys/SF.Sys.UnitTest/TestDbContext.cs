

using Microsoft.EntityFrameworkCore;
using SF.Sys.Data;

namespace SF.Sys.UnitTest
{
	public class TestDbContext : DbContext
	{
		public TestDbContext(DbContextOptions<TestDbContext> options)
			: base(options)
		{
		}


		public override void Dispose()
		{
			base.Dispose();
		}
	}

}
 
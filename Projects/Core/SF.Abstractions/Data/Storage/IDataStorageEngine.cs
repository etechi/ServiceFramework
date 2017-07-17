using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Data.Storage
{
	public enum IsolationLevel
	{
		//
		// 摘要:
		//     正在使用与指定隔离级别不同的隔离级别，但是无法确定该级别。
		Unspecified = -1,
		//
		// 摘要:
		//     无法覆盖隔离级别更高的事务中的挂起的更改。
		Chaos = 16,
		//
		// 摘要:
		//     可以进行脏读，意思是说，不发布共享锁，也不接受独占锁。
		ReadUncommitted = 256,
		//
		// 摘要:
		//     在正在读取数据时保持共享锁，以避免脏读，但是在事务结束之前可以更改数据，从而导致不可重复的读取或幻像数据。
		ReadCommitted = 4096,
		//
		// 摘要:
		//     在查询中使用的所有数据上放置锁，以防止其他用户更新这些数据。防止不可重复的读取，但是仍可以有幻像行。
		RepeatableRead = 65536,
		//
		// 摘要:
		//     在 System.Data.DataSet 上放置范围锁，以防止在事务完成之前由其他用户更新行或向数据集中插入行。
		Serializable = 1048576,
		//
		// 摘要:
		//     通过在一个应用程序正在修改数据时存储另一个应用程序可以读取的相同数据版本来减少阻止。表示您无法从一个事务中看到在其他事务中进行的更改，即便重新查询也是如此。
		Snapshot = 16777216
	}
	[UnmanagedService]
	public interface IDataTransaction:IDisposable
	{
		object UnderlyingTransaction { get; }
		void Commit();
		void Rollback();
	}
	[UnmanagedService]
	public interface IDataStorageEngine
	{
        Task<object> ExecuteCommandAsync(string Sql, CancellationToken CancellationToken,params object[] Args);
		IDataTransaction BeginTransaction();
		IDataTransaction BeginTransaction(IsolationLevel isolationLevel);
	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace SF.Data
{
	
	public static class TransactionScopeManagerExtensions
	{
		public static async Task<T> UseTransaction<T>(
			this ITransactionScopeManager ScopeManager, 
			string Message,
			Func<ITransactionScope,Task<T>> Callback,
			TransactionScopeMode Mode=TransactionScopeMode.RequireTransaction, 
			IsolationLevel IsolationLevel = IsolationLevel.ReadCommitted
			)
		{
			using(var scope =await ScopeManager.CreateScope(Message,Mode,IsolationLevel))
			{
				var re=await Callback(scope);
				return re;
			}
		}
		public static async Task UseTransaction(
			this ITransactionScopeManager ScopeManager,
			string Message,
			Func<ITransactionScope, Task> Callback,
			TransactionScopeMode Mode = TransactionScopeMode.RequireTransaction,
			IsolationLevel IsolationLevel = IsolationLevel.ReadCommitted
			)
		{
			using (var scope = await ScopeManager.CreateScope(Message, Mode, IsolationLevel))
			{
				await Callback(scope);
			}
		}
	}
  
}

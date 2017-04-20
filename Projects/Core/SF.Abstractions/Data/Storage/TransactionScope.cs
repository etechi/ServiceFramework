using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Storage
{
	public enum TransactionScopeMode
    {
        RequireTransaction,
        RequireNewTransaction,
    }
    public class TransactioinRollbackException : PublicInvalidOperationException
    {
        public TransactioinRollbackException(string Message) : base(Message)
        {

        }
    }

    public interface ITransactionScope : IDisposable
    {
        string Message { get; }
        Task Commit();
        bool IsRollbacking { get; }
    }
    public interface ITransactionScopeManager : IDisposable
    {
        Task<ITransactionScope> CreateScope(string Message,TransactionScopeMode Mode, IsolationLevel IsolationLevel=IsolationLevel.ReadCommitted);
    }

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

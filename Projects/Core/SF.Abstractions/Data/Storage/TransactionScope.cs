using SF.Core.Interception;
using SF.Core.ServiceManagement;
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
	[UnmanagedService]
	public interface ITransactionScopeManager : IDisposable
    {
        Task<ITransactionScope> CreateScope(string Message,TransactionScopeMode Mode, IsolationLevel IsolationLevel=IsolationLevel.ReadCommitted);
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class TransactionScopeAttribute : InterceptAttribute
	{
		public string Message { get; }
		public IsolationLevel Level { get; }
		public TransactionScopeMode Mode { get; }
		public TransactionScopeAttribute(string Message,IsolationLevel Level=IsolationLevel.ReadCommitted, TransactionScopeMode Mode=TransactionScopeMode.RequireTransaction)
		{
			this.Message = Message;
			this.Mode = Mode;
			this.Level = Level;

		}
	}
  
}

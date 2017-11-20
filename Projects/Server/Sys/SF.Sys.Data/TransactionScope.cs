#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Data
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
	public enum PostActionType
	{
		BeforeCommit,
		AfterCommit,
		AfterCommitOrRollback
	}
	public interface ITransactionScope 
	{
		int Level { get; }
		string Message { get; }
		//Task Commit();
		bool IsRollbacking { get; }

	}
	[Flags]
	public enum TransactionCommitNotifyType
	{
		None=0,
		BeforeCommit=1,
		AfterCommit=2,
		Rollback=4
	}
	public interface ITransactionCommitTracker
	{
		TransactionCommitNotifyType TrackNotifyTypes { get; }
		Task Notify(TransactionCommitNotifyType Type, Exception Exception);
	}
	public interface ITransactionScopeManager : IDisposable
    {
		DbTransaction CurrentDbTransaction { get; }
		Task UseTransaction(string Message, Func<ITransactionScope,Task> Action,TransactionScopeMode Mode, IsolationLevel IsolationLevel = IsolationLevel.ReadCommitted);
		void AddCommitTracker(ITransactionCommitTracker Tracker);

		//Task<ITransactionScope> CreateScope(string Message,TransactionScopeMode Mode, IsolationLevel IsolationLevel=IsolationLevel.ReadCommitted);
	}

	//[AttributeUsage(AttributeTargets.Method)]
	//public class TransactionScopeAttribute : InterceptAttribute
	//{
	//	public string Message { get; }
	//	public IsolationLevel Level { get; }
	//	public TransactionScopeMode Mode { get; }
	//	public TransactionScopeAttribute(string Message,IsolationLevel Level=IsolationLevel.ReadCommitted, TransactionScopeMode Mode=TransactionScopeMode.RequireTransaction)
	//	{
	//		this.Message = Message;
	//		this.Mode = Mode;
	//		this.Level = Level;

	//	}
	//}
  
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	
    public class TransactionScopeManager : ITransactionScopeManager
    {
		DbConnection Connection { get; }

		public DbTransaction CurrentDbTransaction => _Transaction;

		public TransactionScopeManager(DbConnection Connection)
        {
            this.Connection = Connection;
        }

		class ActionItem
		{
			public PostActionType ActionType { get; set; }
			public Action Func { get; set; }
			public Func<Task> AsyncFunc { get; set; }
		}
		class Scope : ITransactionScope
        {
            public TransactionScopeManager Manager { get; }
            public Scope PrevScope { get; }
            public string Message { get; }
            bool _Completed;
            public bool IsRollbacking { get { return Manager._IsRollbacking; } }
			List<ActionItem> _PostActions;

			public Scope(TransactionScopeManager Manager, Scope PrevScope,string Message)
            {
                this.Manager = Manager;
                this.PrevScope = PrevScope;
                this.Message = Message;
            }
            public async Task Commit()
            {
                if (_Completed)
                    throw new InvalidOperationException();
                if (Manager._TopScope != this)
                    throw new InvalidCastException();
				if (PrevScope == null)
				{
					await ExecutePostActionsAsync(PostActionType.BeforeCommit);
					Manager._Transaction.Commit();
					await ExecutePostActionsAsync(PostActionType.AfterCommit);
				}
                _Completed = true;
            }

            public void Dispose()
            {
                if (Manager._TopScope != this)
                    throw new InvalidCastException();

                Manager._TopScope = PrevScope;
                if (Manager._TopScope == null)
                {
                    var isRollbacking = Manager._IsRollbacking;
                    Manager._IsRollbacking = false;
                    var trans = Manager._Transaction;
                    Manager._Transaction = null;
                    using (trans)
                    {
						if (!_Completed)
						{
							if (!isRollbacking)
								try
								{
									trans.Rollback();
								}
								catch { }
						}
					}
					if (!_Completed)
						ExecutePostActions(PostActionType.AfterCommitOrRollback);
				}
                else
                {
                    if (_Completed)
                        return;
                    if (!Manager._IsRollbacking)
                    {
                        Manager._IsRollbacking = true;
                        try
                        {
                            Manager._Transaction.Rollback();
						}
                        catch { }
                    }
                }
            }

			public void AddPostAction(
				Action action,
				PostActionType ActionType
				)
			{
				var pas = Manager._TopScope._PostActions;
				if (pas == null)
					Manager._TopScope._PostActions = pas = new List<ActionItem>();
				pas.Add(new ActionItem { Func = action, ActionType = ActionType });
			}
			public void AddPostAction(
			   Func<Task> action,
				PostActionType ActionType
			   )
			{
				var pas = Manager._TopScope._PostActions;
				if (pas == null)
					Manager._TopScope._PostActions = pas = new List<ActionItem>();
				pas.Add(new ActionItem { AsyncFunc = action, ActionType = ActionType });
			}
			async Task ExecutePostActionsAsync(PostActionType ActionType)
			{
				if (_PostActions == null)
					return;
				foreach (var action in _PostActions)
					if (action.ActionType==PostActionType.AfterCommitOrRollback && ActionType!=PostActionType.BeforeCommit ||
						action.ActionType==ActionType
						)
					{
						action.Func?.Invoke();
						if (action.AsyncFunc != null)
							await action.AsyncFunc();
					}
			}
			void ExecutePostActions(PostActionType ActionType)
			{
				if (_PostActions == null)
					return;
				if (_PostActions.Any(a => a.AsyncFunc != null))
				{
					Task.Run(() => ExecutePostActions(ActionType)).Wait();
					return;
				}
				foreach (var action in _PostActions)
					if (action.ActionType == PostActionType.AfterCommitOrRollback && ActionType != PostActionType.BeforeCommit ||
						action.ActionType == ActionType
						)
						action.Func?.Invoke();
			}
		}
		
		DbTransaction _Transaction;
        Scope _TopScope;
        bool _IsRollbacking;
		public ITransactionScope CurrentScope => _TopScope;

		public async Task<ITransactionScope> CreateScope(string Message,TransactionScopeMode Mode,System.Data.IsolationLevel IsolationLevel)
        {
            if (Mode == TransactionScopeMode.RequireNewTransaction && _Transaction != null)
                throw new InvalidOperationException("事务已存在");
			if (_Transaction == null)
			{
				if (Connection.State == System.Data.ConnectionState.Closed)
					await Connection.OpenAsync();
				_Transaction = Connection.BeginTransaction(IsolationLevel);

			}
            _TopScope = new Scope(this, _TopScope, Message);
			return (ITransactionScope)_TopScope;
        }

        public void Dispose()
        {
            if (_Transaction != null)
            {
                var trans = _Transaction;
                _Transaction = null;
                using (trans)
                {
                    trans.Rollback();
                    throw new TransactioinRollbackException(_TopScope?.Message ?? "事务未正常结束");
                }
            }
        }
    }
}

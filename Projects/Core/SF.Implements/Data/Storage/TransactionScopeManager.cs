﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Storage
{
	
    public class TransactionScopeManager : ITransactionScopeManager
    {
		DbConnection Connection { get; }

		public DbTransaction CurrentDbTransaction => _Transaction;

		public TransactionScopeManager(DbConnection Connection)
        {
            this.Connection = Connection;
        }

        class Scope : ITransactionScope
        {
            public TransactionScopeManager Manager { get; }
            public Scope PrevScope { get; }
            public string Message { get; }
            bool _Completed;
            public bool IsRollbacking { get { return Manager._IsRollbacking; } }

            public Scope(TransactionScopeManager Manager, Scope PrevScope,string Message)
            {
                this.Manager = Manager;
                this.PrevScope = PrevScope;
                this.Message = Message;
            }
            public Task Commit()
            {
                if (_Completed)
                    throw new InvalidOperationException();
                if (Manager._TopScope != this)
                    throw new InvalidCastException();
                if(PrevScope==null)
                    Manager._Transaction.Commit();
                _Completed = true;
                return Task.CompletedTask;
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
                        if (!_Completed && !isRollbacking)
                            try
                            {
                                trans.Rollback();
                            }
                            catch { }
                    }
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
        }

        DbTransaction _Transaction;
        Scope _TopScope;
        bool _IsRollbacking;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class Disposable
	{
		class EmptyDisposable : IDisposable
		{
			EmptyDisposable() { }
			public static readonly EmptyDisposable instance = new EmptyDisposable();
			public void Dispose()
			{
			}
		}
		public static IDisposable Empty
		{
			get
			{
				return EmptyDisposable.instance;
			}
		}
		public static bool Release(ref IDisposable disposable)
		{
			var re = System.Threading.Interlocked.Exchange(ref disposable, null);
			if (re == null)
				return false;
			re.Dispose();
			return true;
		}
		public static bool Release<T>(ref T disposable)
			where T : class, IDisposable
		{
			var re = System.Threading.Interlocked.Exchange(ref disposable, null);
			if (re == null)
				return false;
			re.Dispose();
			return true;
		}
        class ActionDisposable : IDisposable
        {
            public Action Callback;
            public void Dispose()
            {
                var cb = System.Threading.Interlocked.Exchange(ref Callback, null);
                if (cb == null)
                    return;
                cb();
            }
        }
        public static IDisposable FromAction(Action Callback)
        {
            return new ActionDisposable { Callback = Callback };
        }
        public static IDisposable Combine(params IDisposable[] disposables)
        {
            return FromAction(() =>{
                foreach (var d in disposables)
                    d.Dispose();
            });
        }
	}

}

using System;

using Microsoft.Extensions.DependencyInjection;

namespace SF.DI.Microsoft
{
	class ScopeFactory : IDIScopeFactory
	{
		class Scope : IDIScope
		{
			public IServiceProvider ServiceProvider => InnerScope.ServiceProvider;
			IServiceScope InnerScope { get; }
			public Scope(IServiceScope InnerScope)
			{
				this.InnerScope = InnerScope;
			}

			public void Dispose()
			{
				InnerScope.Dispose();
			}
		}
		IServiceScopeFactory ServiceScopeFactory { get; }
		public ScopeFactory(IServiceScopeFactory ServiceScopeFactory)
		{
			this.ServiceScopeFactory = ServiceScopeFactory;
		}

		public IDIScope CreateScope()
		{
			return new Scope(ServiceScopeFactory.CreateScope());
		}
	}
}

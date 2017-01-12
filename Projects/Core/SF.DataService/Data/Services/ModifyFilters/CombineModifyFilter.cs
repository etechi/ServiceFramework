using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ServiceProtocol.Data.Entity;
namespace ServiceProtocol.ObjectManager.ModifyFilters
{
    public class CombineModifyFilter : IModifyFilter
    {
        class Context : IFilterContext
        {
            IFilterContext[] ChildContexts { get; }
            public Context(IFilterContext[] ChildContexts)
            {
                this.ChildContexts = ChildContexts;
            }
            public async Task Completed(Exception Exception)
            {
                foreach (var c in ChildContexts)
                    await c.Completed(Exception);
            }

            public void Dispose()
            {
                foreach (var c in ChildContexts)
                    c.Dispose();
            }

            public async Task OnModelLoaded()
            {
                foreach (var c in ChildContexts)
                    await c.OnModelLoaded();
            }
        }
        IModifyFilter[] ChildFilters { get; }
        public CombineModifyFilter(params IModifyFilter[] ChildFilters)
        {
            this.ChildFilters = ChildFilters;
        }
        public async Task<IFilterContext> NewContext(IModifyContext Context)
        {
            var ctxs = new IFilterContext[ChildFilters.Length];
            for (var i = 0; i < ctxs.Length; i++)
                ctxs[i] = await ChildFilters[i].NewContext(Context);
            return new Context(ctxs);
        }
    }
}
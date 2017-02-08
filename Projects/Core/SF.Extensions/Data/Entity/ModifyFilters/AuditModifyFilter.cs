using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ServiceProtocol.Data.Entity;
namespace ServiceProtocol.ObjectManager.ModifyFilters
{
    public class AuditModifyFilter : IModifyFilter
    {
        IAuditService AuditService { get; }
        public AuditModifyFilter(IAuditService AuditService)
        {
            this.AuditService = AuditService;

        }
        class Context : IFilterContext
        {
            IAuditContext AuditContext { get; }
            IModifyContext ModifyContext { get; }
            public Context(IModifyContext ModifyContext, IAuditContext AuditContext)
            {
                this.ModifyContext = ModifyContext;
                this.AuditContext = AuditContext;
            }
            public Task Completed(Exception Exception)
            {
                AuditContext.OwnerId = Convert.ToString(ModifyContext.OwnerId);
                
                var resType = ModifyContext.ResourceType;
                AuditContext.DestId = resType + "-" + Convert.ToString(ModifyContext.Id);
                AuditContext.Resource = resType;
                switch (ModifyContext.Action)
                {
                    case ModifyAction.Create:
                        AuditContext.SetNewState(ModifyContext.Model);
                        AuditContext.Operation = "新建";
                        break;
                    case ModifyAction.Delete:
                        AuditContext.Operation = "删除";
                        break;
                    case ModifyAction.Update:
                        AuditContext.SetNewState(ModifyContext.Model);
                        AuditContext.Operation = "修改";
                        break;
                }

                return AuditContext.Commit(Exception);
            }

            public void Dispose()
            {
                
            }

            public Task OnModelLoaded()
            {
                AuditContext.SetOrgState(ModifyContext.Model);
                return Task.CompletedTask;
            }
        }
        public Task<IFilterContext> NewContext(IModifyContext Context)
        {
            return Task.FromResult(
                (IFilterContext)new Context(Context, AuditService.NewContext())
                );
        }
    }

}
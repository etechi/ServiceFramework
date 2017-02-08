using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ServiceProtocol.Data.Entity;

namespace ServiceProtocol.ObjectManager
{
    public class EntityModifiedEvent : ServiceProtocol.Events.IEvent
    {
        public string Source => "对象实体管理";
        public string Type
        {
            get
            {
                switch (Action)
                {
                    case ModifyAction.Create:
                        return "新建";
                    case ModifyAction.Delete:
                        return "删除";
                    case ModifyAction.Update:
                        return "修改";
                    default:
                        throw new NotSupportedException();
                }
            }
        }
        public ModifyAction Action { get; }
        public string ResourceType { get; }
        public object Id { get; }
        public string EntityId => ResourceType + "-" + Id;
        public object Model { get; }
        public object Editable { get; }

        public EntityModifiedEvent(
            ModifyAction Action,
            string ResourceType,
            object Id,
            object Model,
            object Editable
            )
        {
            this.Action = Action;
            this.ResourceType = ResourceType;
            this.Id = Id;
            this.Model = Model;
            this.Editable = Editable;
        }
    }

    namespace ModifyFilters
    {
        public class EventModifyFilter : IModifyFilter
        {
            public ServiceProtocol.Events.IEventEmitter EventEmitter { get; }
            public EventModifyFilter(ServiceProtocol.Events.IEventEmitter EventEmitter)
            {
                this.EventEmitter = EventEmitter;
            }
            class Context : IFilterContext
            {
                ServiceProtocol.Events.IEventEmitter EventEmitter { get; }
                IModifyContext ModifyContext { get; }
                public Context(IModifyContext ModifyContext, ServiceProtocol.Events.IEventEmitter EventEmitter)
                {
                    this.EventEmitter = EventEmitter;
                    this.ModifyContext = ModifyContext;
                }

                public Task OnModelLoaded()
                {
                    return Task.CompletedTask;
                }

                public async Task Completed(Exception Exception)
                {
                    if (Exception != null)
                        return;
                    await EventEmitter.Emit(
                        new EntityModifiedEvent(
                            ModifyContext.Action,
                            ModifyContext.ResourceType,
                            ModifyContext.Id,
                            ModifyContext.Model,
                            ModifyContext.Editable
                            ),false
                        );
                }

                public void Dispose()
                {
                    
                }
            }
            public Task<IFilterContext> NewContext(IModifyContext Context)
            {
                return Task.FromResult((IFilterContext) new Context(Context, EventEmitter));
            }
        }
    }
}
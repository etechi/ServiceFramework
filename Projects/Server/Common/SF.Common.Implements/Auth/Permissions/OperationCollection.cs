using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.Auth.Storages;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Security.Principal;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.Permissions
{
    public class OperationCollection : IOperationCollection
    {
        Dictionary<string, IOperation> Operations { get; } = new Dictionary<string, IOperation>();
        bool _completed;
        public OperationCollection()
        {
            Add(new Operation("查看", "查看", "允许对对象进行浏览，但不能修改"));
            Add(new Operation("管理", "管理", "允许对对象进行管理,包括浏览"));
        }
        public void Add(IOperation operation)
        {
            if (_completed)
                throw new InvalidOperationException("操作收集已结束，不能再添加新操作");
            Operations.Add(operation.Id, operation);
        }
        public IOperation Get(string Id)
        {
            return Operations.Get(Id);
        }
        public IOperationManager GetManager()
        {
            _completed = true;
            return new OperationManager(Operations);
        }
    }
}

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
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Annotations;

namespace ServiceProtocol.Auth.Permissions
{
    public class ResourceCollection : IResourceCollection
    {
        Dictionary<string, IResource> Resources { get; } = new Dictionary<string, IResource>();
        bool _completed;
        public void Add(IResource res)
        {
            if (res.AvailableOperations.Count == 0)
                throw new ArgumentException("资源没有可用的操作:" + res.Id + ":" + res.Name);
            if (_completed)
                throw new InvalidOperationException("资源收集已结束，不能再添加新资源");
            Resources.Add(res.Id, res);
        }

        public IResource Get(string Id)
        {
            return Resources.Get(Id);
        }
        public IResourceManager GetManager()
        {
            _completed = true;
            return new ResourceManager(Resources);
        }
    }
}

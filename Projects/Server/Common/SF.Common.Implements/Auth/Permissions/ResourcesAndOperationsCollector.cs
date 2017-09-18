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
using ServiceProtocol.DI;

namespace ServiceProtocol.Auth.Permissions
{
    class ResourcesAndOperationsCollector : IDisposable
    {
        public Permissions.OperationCollection Operations { get; } = new Permissions.OperationCollection();
        public Permissions.ResourceCollection Resources { get; } = new Permissions.ResourceCollection();
        public IDIRegister Register { get; }
        public ResourcesAndOperationsCollector(IDIRegister Register)
        {
            this.Register = Register;
            Register.RegisterInstance<Permissions.IOperationCollection>(Operations);
            Register.RegisterInstance<Permissions.IResourceCollection>(Resources);
        }
        public void Dispose()
        {
            Register.RegisterInstance<Permissions.IOperationManager>(Operations.GetManager());
            Register.RegisterInstance<Permissions.IResourceManager>(Resources.GetManager());
        }
    }
}

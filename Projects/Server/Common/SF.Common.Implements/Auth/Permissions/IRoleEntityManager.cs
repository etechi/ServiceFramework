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

namespace ServiceProtocol.Auth
{
    public interface IRoleEntityManager
        : ServiceProtocol.ObjectManager.IServiceObjectManager<string,Models.RoleInternal, Models.RoleInternal>
    {
        Task<Models.RoleInternal[]> List();
	}
}

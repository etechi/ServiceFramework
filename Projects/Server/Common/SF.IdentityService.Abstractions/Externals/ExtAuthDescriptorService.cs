using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.Auth.Storages;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Security.Claims;

namespace ServiceProtocol.Auth.Externals
{

	public interface IExtAuthDescriptorManager<TExtAuthDescriptor>:
		ObjectManager.IServiceObjectManager<string, TExtAuthDescriptor>
		where TExtAuthDescriptor:ExtAuthDescriptor
	{
		Task<TExtAuthDescriptor[]> List();
	}
}

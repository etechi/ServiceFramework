using SF.Metadata;
using SF.Auth;
using SF.Auth.Identity;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Data.Storage;

namespace SF.Users.Members
{
	public class MemberServiceSetting : 
		UserServiceSetting
	{
		public Lazy<IMemberManagementService> ManagementService { get; set; }
		public Lazy<ITransactionScopeManager> TransactionScopeManager { get; set; }
	}

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Bizness.Accounting.Management
{
    public interface IAccountTypeManager:
		IServiceObjectManager<int, AccountTitle>
    {
        Task<AccountTitle[]> List();
    }
}

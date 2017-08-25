using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Bizness.Accounting.Management
{
    public interface IAccountTitleManager:
		ObjectManager.IServiceObjectManager<int, AccountTitle>
    {
        Task<AccountTitle[]> List();
    }
}

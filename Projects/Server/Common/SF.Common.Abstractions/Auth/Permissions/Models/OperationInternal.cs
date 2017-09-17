using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Permissions.Models
{

    [EntityObject("用户授权操作")]
    public class OperationInternal : ObjectEntityBase<string>
    {
    }
}

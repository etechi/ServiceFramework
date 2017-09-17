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
   
    [EntityObject("用户授权")]
    public class ResourceGrantInternal : ObjectEntityBase<string>
    {
        [Display(Name = "资源分组")]
        [ReadOnly(true)]
        [Layout(15)]
        public string Group { get; set; }

        [Comment(Name = "资源项目")]
        [ReadOnly(true)]
        [Layout(20)]
        public override string Name { get; set; }

        [Display(Name = "授权")]
        [Layout(25)]
        [EntityIdent(typeof(IOperationManager),ScopeField=nameof(Id))]
        public string[] OperationIds { get; set; }


    }
}

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

	[EntityObject]
	public class ResourceInternal : ObjectEntityBase<string>
    {
        [Display(Name = "资源分组")]
        [ReadOnly(true)]
        [Layout(15)]
        public string Group { get; set; }

		public OperationInternal[] AvailableOperations { get; set; }
	}
  
}

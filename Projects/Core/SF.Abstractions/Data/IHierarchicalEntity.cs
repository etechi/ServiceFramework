using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public interface IHierarachicalEntity<TKey> 
	{
		TKey ParentId { get; set; }
	}
}

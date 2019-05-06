using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Payments
{
	public class PaymentPlatform: UIObjectEntityBase
	{

	}
	public class PaymentPlatformInternal : PaymentPlatform
	{
		public int Order { get; set; }
	}

}

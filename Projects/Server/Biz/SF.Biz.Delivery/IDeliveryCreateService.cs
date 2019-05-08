using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    public interface IDeliveryCreateService
	{
		Task<DeliveryCreateResult> Create(DeliveryCreateArgument arg);
	}
}

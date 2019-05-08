using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    public interface IDeliveryTransportService
	{
		Task<Transport[]> List();
	}
}

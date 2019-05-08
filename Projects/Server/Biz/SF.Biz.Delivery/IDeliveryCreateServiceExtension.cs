using SF.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    public static class DeliveryCreateServiceExtension
	{
		public static async Task<long> GetAddressSnapshotId(long Id,
			IDeliveryAddressService AddressService,
			IDeliveryAddressSnapshotService SnapshotService
			)
		{
			var srcAddr = await AddressService.FindByIdAsync(Id);
			return await SnapshotService.GetAddressId(srcAddr);
		}
		public static async Task<DeliveryCreateResult> CreateByUserAddress(
			this IDeliveryCreateService CreateService,
			IDeliveryAddressService AddressService,
			IDeliveryAddressSnapshotService SnapshotService,
			DeliveryCreateArgument arg
			)
		{
            if (arg.Items.Any(i => !i.VirtualItem))
            {
                if (!arg.DestAddressId.HasValue || !arg.SourceAddressId.HasValue)
                    throw new PublicArgumentException("发货单中包含实物，需要提供地址");
                arg.DestAddressId = await GetAddressSnapshotId(arg.DestAddressId.Value, AddressService, SnapshotService);
                arg.SourceAddressId = await GetAddressSnapshotId(arg.SourceAddressId.Value, AddressService, SnapshotService);
            }
			return await CreateService.Create(arg);

		}
	}
}

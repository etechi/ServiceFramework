using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
namespace ServiceProtocol.Biz.Delivery.Entity
{
    [DataObjectLoader("发货")]
	public class DeliveryService<TUserKey, TDeliveryInternal, TDeliveryItemInternal, TDelivery, TDeliveryItem, TAddress, TTransport,TLocation>:
		IDeliveryService<TUserKey,TDeliveryInternal,TDeliveryItemInternal> ,
        IDataObjectLoader


		where TUserKey : IEquatable<TUserKey>
		where TDeliveryInternal : DeliveryInternal<TUserKey, TDeliveryItemInternal>,new()
		where TDeliveryItemInternal : DeliveryItemInternal,new()
		where TDelivery : Models.Delivery<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport, TLocation>,new()
		where TDeliveryItem : Models.DeliveryItem<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport, TLocation>,new()
		where TAddress : Models.DeliveryAddressSnapshot
		where TTransport : Models.DeliveryTransport
		where TLocation: Models.DeliveryLocation
	{
		public IDataContext Context { get; }
        public Lazy<IDataObjectResolver> DataObjectResolver { get; }

        public DeliveryService(IDataContext Context, Lazy<IDataObjectResolver> DataObjectResolver)
		{
			this.Context = Context;
            this.DataObjectResolver = DataObjectResolver;

        }
		
		IContextQueryable<TDeliveryInternal> MapModelToPublic(IContextQueryable<TDelivery> query)
		{
			return from m in query
				   let daddr = m.DestAddressSnapshotId.HasValue? m.DestAddressSnapshot:null
				   select new TDeliveryInternal
				   {
					   Id = m.Id,
					   Title = m.Title,
					   SenderId = m.SellerId,
					   ReceiverId = m.BuyerId,
					   SourceAddressId = m.SourceAddressSnapshotId,
					   DestAddressId = m.DestAddressSnapshotId,
					   DestAddress =daddr==null?null: new DeliveryAddressDetail
					   {
						   Address = daddr.Address,
						   ContactName = daddr.ContactName,
						   ContactPhoneNumber = daddr.ContactPhoneNumber,
						   LocationName = daddr.LocationName,
						   LocationId = daddr.LocationId,
						   ZipCode = daddr.ZipCode
					   },
					   TrackEntityIdent = m.TrackEntityIdent,
					   TotalQuantity = m.TotalQuantity,
					   Items = from i in m.Items
							   select new TDeliveryItemInternal
							   {
								   Id = i.Id,
								   DeliveryId=i.DeliveryId,
								   Title = i.Title,
								   PayloadEntityIdent = i.PayloadEntityIdent,
                                   PayloadSpecEntityIdent=i.PayloadSpecEntityIdent,
                                   Quantity = i.Quantity,
                                   VirtualItem=i.VirtualItem
							   },
					   State = m.State,
					   CreatedTime = m.CreatedTime,
					   DeliveryTime = m.DeliveryTime,
					   ReceivedTime = m.ReceivedTime,
                       VirtualItemTokenReadTime=m.VirtualItemTokenReadTime,
                       TransportCode =m.TransportCode,
                       TransportName=m.Transport.Name,
                       VirtualItemData = m.VirtualItemToken,

                       TransportId = m.TransportId,
                       HasObjectItem=m.HasObjectItem,
                       HasVirtualItem=m.HasVirtualItem
				   };
		}
        async Task<TDeliveryInternal[]> PrepareDeliveryInternal(TDeliveryInternal[] items)
        {
            await DataObjectResolver.Value.Fill(
                items,
                it => "用户-" + it.ReceiverId,
                (it, n) => it.ReceiverName = n
                );
            return items;
        }

		public async Task<TDeliveryInternal> Get(int Id)
		{
			var re=await MapModelToPublic(Context.ReadOnly<TDelivery>().Where(d => d.Id == Id)).SingleOrDefaultAsync();
            if (re == null)
                return re;
            await PrepareDeliveryInternal(new[] { re });
            return re;
		}


		static PagingQueryBuilder<TDelivery> pagingQueryBuilder = new PagingQueryBuilder<TDelivery>(
			"time",
			i => i.Add("time", m => m.CreatedTime, true)
			);

		public async Task<QueryResult<TDeliveryInternal>> Query(DeliveryQueryArguments<TUserKey> args, Paging paging)
		{
			var q = (IContextQueryable<TDelivery>)Context.ReadOnly<TDelivery>();
			if (!args.ReceiverId.Equals(default(TUserKey)))
				q = q.Where(m => m.BuyerId.Equals(args.ReceiverId));

			if (args.State != null)
				q = q.Where(m => m.State == args.State);
			if (args.CreateTime != null)
			{
				if (args.CreateTime.Begin!=null)
					q = q.Where(m => m.CreatedTime >= args.CreateTime.Begin);
				if (args.CreateTime.End!=null)
					q = q.Where(m => m.CreatedTime <= args.CreateTime.End);
			}
			if (args.Search != null)
				q = q.Where(m => m.Title.Contains(args.Search));

			var re=await q.ToQueryResultAsync(
				MapModelToPublic,
				PrepareDeliveryInternal,
				pagingQueryBuilder,
				paging
				);
			return re;
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            return await DataObjectLoader.Load(
                Keys,
                id => int.Parse(id[0]),
                id => Get(id),
                async (ids) => {
                    var tmps = await MapModelToPublic(Context.ReadOnly<TDelivery>().Where(a => ids.Contains(a.Id))).ToArrayAsync();
                    return await PrepareDeliveryInternal(tmps);
                }
                );
        }

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
namespace ServiceProtocol.Biz.Delivery.Entity
{
    [DataObjectLoader("发货地址")]
	public class DeliveryAddressService<TUserKey, TDeliveryAddress, TDeliveryAddressEditable,TModel,TLocation> :
		ServiceProtocol.ObjectManager.EntityServiceObjectManager<int, TDeliveryAddress, TDeliveryAddressEditable, TModel>,
		IDeliveryAddressService<TUserKey, TDeliveryAddress, TDeliveryAddressEditable>,
        IDataObjectLoader
		where TModel: Models.DeliveryAddress<TUserKey>,new()
		where TLocation:Models.DeliveryLocation
		where TDeliveryAddress : UserDeliveryAddress<TUserKey>,new()
		where TDeliveryAddressEditable : UserDeliveryAddressEditable<TUserKey>, new()
		where TUserKey :IEquatable<TUserKey>
	{
		public DeliveryAddressService(IDataContext Context,Lazy<IModifyFilter> ModifyFilter) :base(Context, ModifyFilter)
		{
		}

        protected override IContextQueryable<TDeliveryAddress> MapModelToInternal(IContextQueryable<TModel> Query)
		{
			return from q in Query
				   select new TDeliveryAddress
				   {
					   Id = q.Id,
					   Address = q.Address,
					   ContactName = q.ContactName,
					   ContactPhoneNumber = q.ContactPhoneNumber,
					   IsDefaultAddress = q.IsDefaultAddress,
					   UserId = q.OwnerId,
					   ZipCode = q.ZipCode,
					   LocationName = q.LocationName,
					   LocationId=q.LocationId
					};
			
		}

        protected override async Task<TDeliveryAddressEditable> MapModelToEditable(IContextQueryable<TModel> Query)
		{
			var q = from qi in Query
				let city=qi.Location.Parent
				select new TDeliveryAddressEditable
				{
					Id=qi.Id,
					Address = qi.Address,
					ContactName = qi.ContactName,
					ContactPhoneNumber = qi.ContactPhoneNumber,
					IsDefaultAddress = qi.IsDefaultAddress,
					UserId = qi.OwnerId,
					DistrictId=qi.LocationId,
					CityId=city.Id,
					ProvinceId=city.ParentId==null?0: city.ParentId.Value
				};
			return await q.SingleOrDefaultAsync();
		}
		
		protected virtual void OnVerifyDeliveryAddress(TDeliveryAddressEditable Address)
		{
			Ensure.HasContent(Address.ContactName, "收件人");
			Ensure.HasContent(Address.ContactPhoneNumber, "联系电话");
			Ensure.HasContent(Address.Address, "地址");
		//	Ensure.HasContent(Address.ZipCode, "邮编");

		}
		
		protected override async Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			OnVerifyDeliveryAddress(obj);

			var a = await Context.ReadOnly<TLocation>().Where(l => l.Id == obj.DistrictId).FirstOrDefaultAsync();
			if (a == null)
				throw new ArgumentException();

			Model.Address = obj.Address;
			Model.ContactName = obj.ContactName;
			Model.ContactPhoneNumber = obj.ContactPhoneNumber;
			Model.PhoneNumberVerified = obj.PhoneNumberVerified;
			Model.LocationId = obj.DistrictId;
			Model.OwnerId = obj.UserId;

			Model.ZipCode = a.Code;
			if (a.FullName==null)
			{
				var c = await Context.ReadOnly<TLocation>().Where(l => l.Id == a.ParentId).Include(p=>p.Parent).SingleOrDefaultAsync();
				Model.LocationName = c.Parent.Name + c.Name + a.Name;
			}
			else
				Model.LocationName = a.FullName;
			Model.IsDefaultAddress = obj.IsDefaultAddress;
			if (obj.IsDefaultAddress)
			{
				var curs = await Context.Editable<TModel>().Where(m => m.OwnerId.Equals(obj.UserId) && m.IsDefaultAddress).ToArrayAsync();
				if (curs.Length>0)
				{
					foreach (var c in curs)
					{
						c.IsDefaultAddress = false;
						Context.Update(c);
					}
				}
			}
			else
			{
				if (!await Context.ReadOnly<TModel>().AnyAsync(m => m.OwnerId.Equals(obj.UserId) && m.Id!=Model.Id && m.IsDefaultAddress))
					Model.IsDefaultAddress = true;
			}
			
		}
        protected override async Task OnRemoveModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			if (Model.IsDefaultAddress)
			{
				var cur = await Context.Editable<TModel>().Where(m => m.OwnerId.Equals(Model.OwnerId) && m.Id != Model.Id).FirstOrDefaultAsync();
				if (cur != null)
				{
					cur.IsDefaultAddress = true;
					Context.Update(cur);
				}
			}
			await base.OnRemoveModel(ctx);
		}

		public async Task<TDeliveryAddress> GetUserDefaultAddress(TUserKey UserId)
		{
			return await MapModelToInternal(
				Context.ReadOnly<TModel>().Where(m => m.OwnerId.Equals(UserId) && m.IsDefaultAddress))
				.FirstOrDefaultAsync();
		}
		public async Task<TDeliveryAddress[]> ListUserAddresses(TUserKey UserId)
		{
			return await MapModelToInternal(
				Context.ReadOnly<TModel>().Where(m => m.OwnerId.Equals(UserId)))
				.ToArrayAsync();
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            return await DataObjectLoader.Load(
                Keys,
                id => int.Parse(id[0]),
                id => FindByIdAsync(id),
                async (ids) => {
                    var tmps = await MapModelToInternal(
                        Context.ReadOnly<TModel>().Where(a => ids.Contains(a.Id))
                        ).ToArrayAsync();
                    return await OnPrepareInternals(tmps);
                }
                );
        }
    }
}

using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Entities;
using System.Linq;
using SF.Sys;
using System.Text;
using System.Collections.Generic;

namespace SF.Biz.Delivery.Management
{
    public class UserDeliveryAddressManager:
        AutoModifiableEntityManager<ObjectKey<long>, UserDeliveryAddress, UserDeliveryAddress, UserDeliveryAddressQueryArguments, UserDeliveryAddress, DataModels.DataDeliveryAddress>,
        IUserDeliveryAddressManager

    {
        public UserDeliveryAddressManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }

        protected virtual void OnCollectDeliveryAddressContent(
             StringBuilder builder,
             UserDeliveryAddress Address,
             string LocationName,
             string ZipCode
             )
        {
            builder.AppendLine(Address.ContactName);
            builder.AppendLine(Address.ContactPhoneNumber);
            builder.AppendLine(Address.DistrictId.ToString());
            builder.AppendLine(Address.Address);
            builder.AppendLine(LocationName);
            builder.AppendLine(ZipCode);
        }
        protected virtual void OnVerifyDeliveryAddress(UserDeliveryAddress Address)
        {
            Ensure.HasContent(Address.ContactName, "收件人");
            Ensure.HasContent(Address.ContactPhoneNumber, "联系电话");
            Ensure.HasContent(Address.Address, "地址");
            Ensure.NotDefault(Address.DistrictId, "地区");

        }

        async Task<KeyValuePair<string, string>> FindLocationFullNameAndCode(IDataContext ctx,long LocationId, string CurName, string Code)
        {
            var loc = await ctx.Queryable<DataModels.DataDeliveryLocation>().Where(l => l.Id == LocationId).SingleOrDefaultAsync();
            if (loc == null)
                throw new PublicArgumentException("地区ID不正确:" + LocationId);
            if ((loc.FullName != null || loc.Level == 1) && (Code != null || loc.Code != null))
                return new KeyValuePair<string, string>((loc.FullName ?? loc.Name) + CurName, Code ?? loc.Code);

            return await FindLocationFullNameAndCode(ctx,loc.ParentId.Value, loc.Name + CurName, loc.Code);
        }
        public async Task<long> GetSnapshot(long Id)
        {
            var Address = await GetAsync(Id);

            Ensure.NotNull(Address, nameof(Address));

            OnVerifyDeliveryAddress(Address);

            return await DataScope.Use("生成快照", async ctx =>
             {
                 var loc = await FindLocationFullNameAndCode(ctx, Address.DistrictId, "", null);
                 if (string.IsNullOrEmpty(loc.Value))
                     loc = new KeyValuePair<string, string>(loc.Key, Address.DistrictId.ToString());


                 var sb = new StringBuilder();
                 OnCollectDeliveryAddressContent(sb, Address, loc.Key, loc.Value);
                 var hash = sb.ToString().UTF8Bytes().MD5().Base64();

                 var try_count = 5;
                 for (; ; )
                 {
                     var id = await ctx.Queryable<DataModels.DataDeliveryAddressSnapshot>().Where(a => a.Hash == hash).Select(a => a.Id).SingleOrDefaultAsync();
                     if (id != 0)
                         return id;
                     var m = new DataModels.DataDeliveryAddressSnapshot
                     {
                         Hash = hash,
                         Address = Address.Address,
                         ContactName = Address.ContactName,
                         ContactPhoneNumber = Address.ContactPhoneNumber,
                         LocationId = Address.DistrictId,
                         LocationName = loc.Key,
                         ZipCode = loc.Value
                     };
                     ctx.Add(m);
                     try
                     {
                         await ctx.SaveChangesAsync();
                         return m.Id;
                     }
                     catch
                     {
                         try_count--;
                         if (try_count <= 0)
                             throw;
                         ctx.ClearTrackingEntities();
                     }
                 }
             });
        }

        public async Task<DeliveryAddressDetail> QueryShapshotAddress(long Id)
        {
            var re=await DataScope.Use("查询用户地址快照", ctx =>
                (from a in ctx.Queryable<DataModels.DataDeliveryAddressSnapshot>()
                 where a.Id == Id
                 select new DeliveryAddressDetail
                 {
                     Address = a.Address,
                     ContactName = a.ContactName,
                     ContactPhoneNumber = a.ContactPhoneNumber,
                     LocationId = a.LocationId,
                     LocationName = a.LocationName,
                     ZipCode = a.ZipCode
                 }
                 ).SingleOrDefaultAsync()
            );
            if (re == null)
                throw new PublicArgumentException("找不到收货地址:"+Id);
            return re;
        }
    }

}

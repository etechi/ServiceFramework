#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0


using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys;
using System.Net.Http;
using SF.Sys.Data;
using System.Linq;
using SF.Sys.Settings;
using System;

namespace SF.Common.ShortLinks
{
    /// <summary>
    /// 短链接管理
    /// </summary>
	public class ShortLinkManager :
		AutoModifiableEntityManager<ObjectKey<string>, ShortLink, ShortLink, ShortLinkQueryArguments, ShortLink, DataShortLink>,
		IShortLinkManager,
		IShortLinkCreateService,
		IShortLinkService
	{
		ISettingService<HttpSetting> Setting { get; }
		public ShortLinkManager(
			IEntityServiceContext ServiceContext,
			ISettingService<HttpSetting> Setting
			) : base(ServiceContext)
		{
			this.Setting = Setting;
		}

		public async Task<string> Create(ShortLinkCreateArgument Arg)
		{
			var id = Arg.Target.UTF8Bytes().MD5().Hex();
			var editable=await LoadForEdit(ObjectKey.From(id));
			if (editable != null)
			{
				editable.Name = Arg.Name;
				editable.Target = Arg.Target;
				editable.ExpireTime = Arg.Expires;
				await UpdateAsync(editable);
			}
			else
			{
				await CreateAsync(new ShortLink
				{
					Name = Arg.Name,
					Target = Arg.Target,
					Id = id,
					ExpireTime = Arg.Expires
				});
			}
			return new Uri(new Uri(Setting.Value.GetApiUrlBase()), "shortlink/go/" + id).ToString();
		}
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			if (ctx.Editable.PostData != null)
				throw new PublicNotSupportedException("不支持Post格式");
			return base.OnUpdateModel(ctx);
		}
		public async Task<HttpResponseMessage> Go(string Id)
		{
			var re = await DataScope.Use("查询短链接", ctx =>
			  {
				  var now = Now;
				  return (
					  from l in ctx.Queryable<DataShortLink>()
					  where l.Id == Id &&
							  l.LogicState == EntityLogicState.Enabled &&
							  l.ExpireTime > now
					  select new
					  {
						  l.Target,
						  l.PostData
					  }).SingleOrDefaultAsync();
			  });
			if (re == null)
				return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
			var resp = new HttpResponseMessage(System.Net.HttpStatusCode.Found);
			resp.Headers.Location = new System.Uri(re.Target);
			return resp;
		}
	}

}

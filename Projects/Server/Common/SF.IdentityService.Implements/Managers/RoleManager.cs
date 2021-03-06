﻿#region Apache License Version 2.0
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


using System.Linq;
using System.Threading.Tasks;
using SF.Auth.IdentityServices.DataModels;
using SF.Auth.IdentityServices.Models;
using SF.Sys.Entities;

namespace SF.Auth.IdentityServices.Managers
{
	public class RoleManager:
		//QuerableEntitySource<long, Models.IdentityInternal, IdentityQueryArgument, TIdentity>,
		AutoModifiableEntityManager<
			ObjectKey<string>,
			Models.Role,
			Models.Role,
			RoleQueryArgument,
			Models.RoleEditable,
			DataModels.DataRole
			>,
		IRoleManager
	{
		public RoleManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
		//public override async Task<RoleEditable> LoadForEdit(ObjectKey<string> Key)
		//{
		//	var re=await base.LoadForEdit(Key);
		//	if (re == null) return null;
		//	//re.GrantEditables = re.Grants.GroupBy(g => g.ResourceId).Select(g => new GrantEditable
		//	//{
		//	//	ResourceId = g.Key,
		//	//	OperationIds = g.Select(gi => gi.OperationId)
		//	//});
		//	re.Grants = null;
		//	return re;
		//}
		//protected override Task OnUpdateModel(IModifyContext ctx)
		//{
		//	var e = ctx.Editable;
		//	//if (e.GrantEditables != null)
		//	//{
		//	//	e.Grants = from ge in e.GrantEditables
		//	//			   from oid in ge.OperationIds
		//	//			   select new Grant
		//	//			   {
		//	//				   ResourceId = ge.ResourceId,
		//	//				   OperationId = oid
		//	//			   };
		//	//}

		//	return base.OnUpdateModel(ctx);
		//}

		//protected override async Task OnUpdateModel(IModifyContext ctx)
		//{
		//	var e = ctx.Editable;
		//	var m = ctx.Model;


		//	if (e.Claims != null)
		//	{
		//		var ccs = DataSet.Context.Set<DataModels.RoleClaimValue>();
		//		var oids = ccs.AsQueryable(false).Where(c => c.RoleId == m.Id).ToArray();

		//		foreach (var c in e.Claims)
		//		{
		//			if (c.Id == 0)
		//				c.Id = await IdentGenerator.GenerateAsync(typeof(DataModels.RoleClaimValue).FullName);
		//			//if(c.TypeId==0)
		//			//c.TypeId = await ServiceContext.GetOrCreateClaimType(c.TypeName);
		//		}

		//		ccs.Merge(
		//			oids,
		//			e.Claims,
		//			(o, n) => o.Id == n.Id,
		//			n => new DataModels.RoleClaimValue
		//			{
		//				Id = n.Id,
		//				RoleId = m.Id,
		//				TypeId = n.TypeId,
		//				CreateTime = Now,
		//				UpdateTime = Now,
		//				Value = n.Value
		//			},
		//			(o, n) =>
		//			{
		//				o.Value = n.Value;
		//				o.UpdateTime = Now;
		//			}
		//			);
		//	}
		//}

		//protected override async Task OnRemoveModel(IModifyContext ctx)
		//{
		//	var cvs = DataSet.Context.Set<DataModels.RoleClaimValue>();
		//	await cvs.RemoveRangeAsync(ic => ic.RoleId == ctx.Model.Id);

		//	await base.OnRemoveModel(ctx);
		//}

	}

}

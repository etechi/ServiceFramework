using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Entities;
using SF.Data;
using SF.Auth.Permissions.DataModels;
using SF.Auth.Permissions.Models;

namespace SF.Auth.Permissions
{
	public class GrantManager :
		GrantManager<
			Models.GrantEditable,
			DataModels.Grant,
			DataModels.Role, 
			DataModels.RolePermission, 
			DataModels.GrantRole, 
			DataModels.GrantPermission
			>,
		IGrantManager
	{
		public GrantManager(
			IDataSetEntityManager<DataModels.Grant> EntityManager, 
			Lazy<IOperationManager> OperationManager, 
			Lazy<IResourceManager> ResourceManager) 
			: base(EntityManager, OperationManager, ResourceManager)
		{
		}
	}

	public class GrantManager<TGrantEditable, TGrant,TRole, TRolePermission, TGrantRole, TGrantPermission> :
        EntityManager<long, TGrantEditable, GrantQueryArgument, TGrantEditable, TGrant>,
        IGrantManager<TGrantEditable>

		where TGrant: DataModels.Grant<TGrant,TRole, TGrantRole, TRolePermission, TGrantPermission>, new()
	    where TRole : DataModels.Role<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRolePermission : DataModels.RolePermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TGrantRole : DataModels.GrantRole<TGrant, TRole,  TGrantRole, TRolePermission, TGrantPermission>, new()
		where TGrantPermission : DataModels.GrantPermission<TGrant, TRole,  TGrantRole, TRolePermission, TGrantPermission>, new()

		where TGrantEditable : Models.GrantEditable, new()
	{
		public Lazy<IOperationManager> OperationManager { get; }
        public Lazy<IResourceManager> ResourceManager { get; }
        public GrantManager(
            IDataSetEntityManager<TGrant> EntityManager,
            Lazy<IOperationManager> OperationManager,
            Lazy<IResourceManager> ResourceManager
            ) : base(EntityManager)
        {
            this.OperationManager = OperationManager;
            this.ResourceManager = ResourceManager;
        }

		protected override IContextQueryable<TGrant> OnBuildQuery(IContextQueryable<TGrant> Query, GrantQueryArgument Arg, Paging paging)
		{

			return Query.Filter(Arg.Name, r => r.Name)
				.Filter(Arg.LogicState, r => r.LogicState);
		}
		protected override PagingQueryBuilder<TGrant> PagingQueryBuilder { get; } = new PagingQueryBuilder<TGrant>(
			"id",
			b => b.Add("id", r => r.Name )
			);

		protected override IContextQueryable<TGrantEditable> OnMapModelToInternal(IContextQueryable<TGrant> Query)
        {
            return from r in Query
                   select new TGrantEditable
				   {
					   Id = r.Id,
					   Name = r.Name,
					   LogicState = r.LogicState,
					   CreatedTime = r.CreatedTime,
					   UpdatedTime = r.UpdatedTime,
					   InternalRemarks = r.InternalRemarks
				   };
		}

		protected override IContextQueryable<TGrant> OnLoadChildObjectsForUpdate(long Id, IContextQueryable<TGrant> query)
		{
			return base.OnLoadChildObjectsForUpdate(
				Id, 
				query.Include(g=>g.Roles).Include(g=>g.Permissions)
				);
		}
		protected override async Task<TGrantEditable> OnMapModelToEditable(IContextQueryable<TGrant> Query)
        {
			var re=await (from g in Query
				   select new {
					   grant = new TGrantEditable
					   {
						   Id = g.Id,
						   Name = g.Name,
						   LogicState = g.LogicState,
						   CreatedTime = g.CreatedTime,
						   UpdatedTime = g.UpdatedTime,
						   InternalRemarks = g.InternalRemarks,

						   Roles = g.Roles.Select(r => r.RoleId),
					},
					   permissions = from p in g.Permissions select new { op = p.OperationId, res = p.ResourceId }
				   }).SingleOrDefaultAsync();

			re.grant.ResGrants = re.permissions.GroupBy(p => p.res).Select(
				g =>
				{
					var res = ResourceManager.Value.GetAsync(g.Key).Result;
					return new ResourceGrantInternal
					{
						Id = g.Key,
						Group = res.Group,
						Name = res.Name,
						OperationIds = g.Select(gi => gi.op).ToArray()
					};
				}).ToArray();
			return re.grant;
        }
        protected override Task OnNewModel(IModifyContext ctx)
        {
			//ctx.Model.IsAdminRole = true;
			ctx.Model.Id = ctx.Editable.Id;
			ctx.Model.CreatedTime = Now;
			return base.OnNewModel(ctx);
        }
        protected override async Task OnUpdateModel(IModifyContext ctx)
        {
            var m = ctx.Model;
            var e = ctx.Editable;
           
			m.LogicState = e.LogicState;

			m.UpdatedTime = Now;
			m.InternalRemarks = e.InternalRemarks;


			if (e.ResGrants!=null)
                foreach(var g in e.ResGrants)
                {
                    var ops = await ResourceManager.Value.GetResourceOperations(g.Id);
                    if (ops == null || ops.Length==0) 
                        throw new PublicArgumentException("找不到资源:" + g.Id);
                    foreach (var o in g.OperationIds)
                        if (!ops.Any(t => t.Id == o))
                            throw new PublicArgumentException($"资源{g.Id} 找不到操作:{o}");
                }


            var nps = e.ResGrants.SelectMany(r =>
                r.OperationIds,
                (r, o) => new TGrantPermission { GrantId = m.Id, OperationId = o, ResourceId = r.Id }
                )
                .ToArray();
            var re=DataContext.Set<TGrantPermission>().Merge(
                m.Permissions,
                nps,
                (o, n) => o.OperationId == n.OperationId && o.ResourceId == n.ResourceId,
                n => n
                );

			DataContext.Set<TGrantRole>().Merge(
				m.Roles,
				e.Roles,
				(o, n) => o.RoleId == n,
				n => new TGrantRole { GrantId = m.Id, RoleId = n }
				);
        }

        protected override Task OnRemoveModel(IModifyContext ctx)
        {
            DataContext.Set<TGrantPermission>().RemoveRange(ctx.Model.Permissions);
			DataContext.Set<TGrantRole>().RemoveRange(ctx.Model.Roles);
			return base.OnRemoveModel(ctx);
        }
    }
}

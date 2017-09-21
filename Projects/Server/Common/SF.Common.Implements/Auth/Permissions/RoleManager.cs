using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Entities;
using SF.Data;
using SF.Auth.Permissions.DataModels;

namespace SF.Auth.Permissions
{
	public class RoleManager :
		RoleManager<
			Models.RoleInternal,
			RoleQueryArgument,
			DataModels.Grant, 
			DataModels.Role, 
			DataModels.RolePermission, 
			DataModels.GrantRole, 
			DataModels.GrantPermission
			>,
		IRoleManager
	{
		public RoleManager(IDataSetEntityManager<Role> EntityManager, Lazy<IOperationManager> OperationManager, Lazy<IResourceManager> ResourceManager) : base(EntityManager, OperationManager, ResourceManager)
		{
		}
	}

	public class RoleManager<TRoleInternal, TQueryArgument, TGrant,TRole, TRolePermission, TGrantRole, TGrantPermission> :
        EntityManager<string, TRoleInternal, TQueryArgument, TRoleInternal,TRole>,
        IRoleManager<TRoleInternal, TQueryArgument>
		where TGrant : DataModels.Grant<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>
		where TRole : DataModels.Role<TGrant,TRole, TGrantRole, TRolePermission, TGrantPermission>,new()
		where TRolePermission : DataModels.RolePermission<TGrant, TRole, TGrantRole, TRolePermission, TGrantPermission>,new()
		where TGrantRole : DataModels.GrantRole<TGrant, TRole,  TGrantRole, TRolePermission, TGrantPermission>
		where TGrantPermission : DataModels.GrantPermission<TGrant, TRole,  TGrantRole, TRolePermission, TGrantPermission>

		where TRoleInternal:Models.RoleInternal,new()
		where TQueryArgument : RoleQueryArgument,new()
	{
		public Lazy<IOperationManager> OperationManager { get; }
        public Lazy<IResourceManager> ResourceManager { get; }
        public RoleManager(
            IDataSetEntityManager<TRole> EntityManager,
            Lazy<IOperationManager> OperationManager,
            Lazy<IResourceManager> ResourceManager
            ) : base(EntityManager)
        {
            this.OperationManager = OperationManager;
            this.ResourceManager = ResourceManager;
        }

		protected override IContextQueryable<TRole> OnBuildQuery(IContextQueryable<TRole> Query, TQueryArgument Arg, Paging paging)
		{

			return Query.Filter(Arg.Name, r => r.Name)
				.Filter(Arg.LogicState, r => r.LogicState);
		}
		protected override PagingQueryBuilder<TRole> PagingQueryBuilder { get; } = new PagingQueryBuilder<TRole>(
			"name",
			b => b.Add("name", r => r.Name)
			);

		protected override IContextQueryable<TRoleInternal> OnMapModelToDetail(IContextQueryable<TRole> Query)
        {
            return from r in Query
                   select new TRoleInternal
				   {
					   Id = r.Id,
					   Name = r.Name,
					   LogicState = r.LogicState,
					   CreatedTime = r.CreatedTime,

					   UpdatedTime = r.UpdatedTime,
					   InternalRemarks = r.InternalRemarks
				   };
		}
        protected override async Task<TRoleInternal> OnMapModelToEditable(IContextQueryable<TRole> Query)
        {
            var re = await OnMapModelToDetail(Query).SingleOrDefaultAsync();
            if (re == null)
                return null;


            var permissions = (await DataContext.Set<TRolePermission>().AsQueryable()
                .Where(p => p.RoleId==re.Id).ToArrayAsync())
                .GroupBy(p => p.ResourceId).ToDictionary(g => g.Key, g => g.Select(p => p.OperationId).ToArray());

            re.Grants = (await ResourceManager.Value.QueryAsync(new TQueryArgument(),Paging.All)).Items.Select(r => new Models.ResourceGrantInternal
            {
                Id = r.Id,
                Group=r.Group,
                Name = r.Name,
                OperationIds = permissions.Get(r.Id) ?? Array.Empty<string>()
            }).ToArray();

            return re;
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
            UIEnsure.HasContent(e.Id, "需要输入角色ID");
            UIEnsure.HasContent(e.Name, "需要输入角色名称");
			m.Name = e.Name;
			m.LogicState = e.LogicState;

			m.UpdatedTime = Now;
			m.InternalRemarks = e.InternalRemarks;


			if (e.Grants!=null)
                foreach(var g in e.Grants)
                {
                    var ops = await ResourceManager.Value.GetResourceOperations(g.Id);
                    if (ops == null || ops.Length==0) 
                        throw new PublicArgumentException("找不到资源:" + g.Id);
                    foreach (var o in g.OperationIds)
                        if (!ops.Any(t => t.Id == o))
                            throw new PublicArgumentException($"资源{g.Id} 找不到操作:{o}");
                }


            var nps = e.Grants.SelectMany(r =>
                r.OperationIds,
                (r, o) => new TRolePermission { RoleId = m.Id, OperationId = o, ResourceId = r.Id }
                )
                .ToArray();
            var re=DataContext.Set<TRolePermission>().Merge(
                m.Permissions,
                nps,
                (o, n) => o.OperationId == n.OperationId && o.ResourceId == n.ResourceId,
                n => n
                );
        }

        protected override Task OnRemoveModel(IModifyContext ctx)
        {
            DataContext.Set<TRolePermission>().RemoveRange(ctx.Model.Permissions);
            return base.OnRemoveModel(ctx);
        }
        protected override IContextQueryable<TRole> OnLoadChildObjectsForUpdate(string Id, IContextQueryable<TRole> query)
        {
            return query.Include(u => u.Permissions);
        }
       
    }
}

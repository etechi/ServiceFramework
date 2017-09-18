using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Entities;
using SF.Data;

namespace SF.Auth.Permissions
{
	public class RoleManager: RoleManager<Models.RoleInternal,RoleQueryArgument,DataModels.Role, DataModels.RolePermission, DataModels.IdentityRole, DataModels.IdentityPermission>
	{

	}

	public class RoleManager<TRoleInternal, TQueryArgument, TRole, TRolePermission, TIdentityRole, TIdentityPermission> :
        EntityManager<string, TRoleInternal, TQueryArgument, TRoleInternal,TRole>,
        IRoleManager<TRoleInternal, TQueryArgument>

        where TRole : DataModels.Role<TRole, TRolePermission, TIdentityRole, TIdentityPermission>,new()
		where TRolePermission : DataModels.RolePermission<TRole, TRolePermission, TIdentityRole, TIdentityPermission>,new()
		where TIdentityRole : DataModels.IdentityRole<TRole, TRolePermission, TIdentityRole, TIdentityPermission>
		where TIdentityPermission : DataModels.IdentityPermission<TRole, TRolePermission, TIdentityRole, TIdentityPermission>

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


        protected override IContextQueryable<TRoleInternal> OnMapModelToInternal(IContextQueryable<TRole> Query)
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
            var re = await OnMapModelToInternal(Query).SingleOrDefaultAsync();
            if (re == null)
                return null;


            var permissions = (await DataContext.Set<TRolePermission>().AsQueryable()
                .Where(p => p.RoleId==re.Id).ToArrayAsync())
                .GroupBy(p => p.ResourceId).ToDictionary(g => g.Key, g => g.Select(p => p.OperationId).ToArray());

            re.Grants = (await ResourceManager.Value.QueryAsync(new TQueryArgument(),Paging.All)).Items.Select(r => new Models.ResourceGrantInternal
            {
                Id = r.Id,
                CreatedTime = r.CreatedTime,
				InternalRemarks=r.InternalRemarks,
				LogicState=r.LogicState,
				UpdatedTime=r.UpdatedTime,
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
			m.CreatedTime = Now;
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

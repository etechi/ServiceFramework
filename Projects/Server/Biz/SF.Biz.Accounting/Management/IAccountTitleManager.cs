using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;

namespace SF.Biz.Accounting
{    
    /// <summary>
     /// 科目管理器
     /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.财务专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface IAccountTitleManager:
        IEntityManager<ObjectKey<long>, AccountTitle>,
        IEntitySource<ObjectKey<long>, AccountTitle, ObjectQueryArgument>

    {
    }
}

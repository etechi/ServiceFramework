using SF.Sys.Entities;

namespace SF.Biz.Accounting
{
    public interface IAccountTitleManager:
        IEntityManager<ObjectKey<long>, AccountTitle>,
        IEntitySource<ObjectKey<long>, AccountTitle, ObjectQueryArgument>

    {
    }
}

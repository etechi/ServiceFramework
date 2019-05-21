using System;
using System.Threading.Tasks;

namespace SF.Biz.ShoppingCarts
{
    public class ModifyArguments
    {
        public string Type { get; set; }
        public string[] Items { get; set; }
        public int Quantity { get; set; }
    }
    public class QueryArguments
    {
        public string Type { get; set; }
        public bool Selected { get; set; }
        public bool Enabled { get; set; }
        public bool IgnoreQuantityCheck { get; set; }
    }
    public class SyncArguments
    {
        public string Type { get; set; }
        public ItemStatus[] Items { get; set; }
        public bool SkipLimitEnsure { get; set; }
    }


    public interface IShoppingCartService
    {
        Task<int> Count(string Type);
        Task<ShoppingCartItem[]> List(QueryArguments Args);
        Task<int> Update(ModifyArguments Args);
        Task<int> Add(ModifyArguments Args);
        Task<int> TryAdd(ModifyArguments Args);
        Task<int> Remove(ModifyArguments Args);
        Task Sync(SyncArguments Args);
        Task Clear(string Type,bool Selected);
        Task<long> CreateTrade(string Type);

    }
}

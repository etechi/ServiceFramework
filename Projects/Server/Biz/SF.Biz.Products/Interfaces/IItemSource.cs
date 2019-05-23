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

namespace SF.Biz.Products
{
	public interface IBatchLoader<T>
	{
		Task<T[]> Load(long[] Ids);
	}
	public interface IRelationLoader<T>
	{
		Task<long[]> Load(T Id);
	}
    public interface IRelationLoader : IRelationLoader<long>
    {
        
    }

    public interface IItemNotifier
	{
		void NotifyProductChanged(long ProductId);
		void NotifyItemChanged(long ItemId);
		void NotifyCategoryChanged(long CategoryId);
		void NotifyCategoryChildrenChanged(long CategoryId);
		void NotifyCategoryItemsChanged(long CategoryId);
        void NotifyCategoryTag(string CategoryTag);
	}
	public interface IItemSource
	{
        long MainCategoryId { get;}
		IBatchLoader<IItemCached> ItemLoader { get; }
		IBatchLoader<ICategoryCached> CategoryLoader { get; }
		IBatchLoader<IProductCached> ProductLoader { get; }
		IRelationLoader CategoryChildrenLoader { get; }
		IRelationLoader CategoryItemsLoader { get; }
        IRelationLoader<string> TaggedCategoryLoader { get; }
    }
	
}

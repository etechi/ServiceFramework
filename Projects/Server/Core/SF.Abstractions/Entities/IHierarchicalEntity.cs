using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IItemEntity<TContainerKey>
	{
		TContainerKey ContainerId { get; set; }
		//string ContainerName { get; set; }
	}

	public interface IItemEntity : 
		IItemEntity<long?>
	{
	}

	public interface IItemEntityOrder
	{
		int ItemOrder { get; set; }
	}
	public interface IContainerEntity<TContainerKey,TItem,TItemContainerKey>
		where TItem: IItemEntity<TItemContainerKey>
		where TContainerKey:IEquatable<TContainerKey>
	{
		TContainerKey Id { get; set; }
		IEnumerable<TItem> Items { get; set; }
	}
	public interface IContainerEntity<TItem>:
		IContainerEntity<long,TItem,long?>
		where TItem:IItemEntity
	{
	}

	public interface IContainerLoadable<TKey,TContainer>
	{
		[Comment("通过ID获取容器对象")]
		Task<TContainer> LoadContainerAsync(TKey Key);
	}
	public interface IContainerItemsListable<TContainerKey, TItem>
	{
		[Comment("获取容器中的对象")]
		Task<QueryResult<TItem>> ListItemsAsync(TContainerKey Container,Paging Paging);
	}

	public interface ITreeNodeEntity<TNodeEntity,TTreeNodeKey,TTreeParentKey>:
		IItemEntity<TTreeParentKey>,
		IItemEntityOrder
		where TTreeNodeKey:IEquatable<TTreeNodeKey>
		where TNodeEntity: ITreeNodeEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey>
	{
		TTreeNodeKey Id { get; set; }
		IEnumerable<TNodeEntity> Children { get; set; }
	}

	public interface ITreeNodeEntity<TNodeEntity> :
		ITreeNodeEntity<TNodeEntity, long, long?>,
		IItemEntity
		where TNodeEntity : ITreeNodeEntity<TNodeEntity>
	{ }

	[Comment("获取容器中的子容器")]
	public interface ITreeContainerListable<TContainerKey, TContainer>
	{
		Task<QueryResult<TContainer>> ListChildContainersAsync(TContainerKey Key,Paging Paging);
	}

	public interface ITreeContainerEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey, TItemEntity, TItemContainerKey> :
		ITreeNodeEntity<TNodeEntity,TTreeNodeKey,TTreeParentKey>,
		IContainerEntity<TTreeNodeKey,TItemEntity,TItemContainerKey>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TNodeEntity : ITreeContainerEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey, TItemEntity, TItemContainerKey>
		where TItemEntity: IItemEntity<TItemContainerKey>
	{

	}
	public interface ITreeContainerEntity<TTreeContainerEntity, TItemEntity> :
		ITreeContainerEntity<TTreeContainerEntity, long, long?, TItemEntity, long?>
		where TTreeContainerEntity : ITreeContainerEntity<TTreeContainerEntity, TItemEntity>
		where TItemEntity : IItemEntity
	{
	}

}

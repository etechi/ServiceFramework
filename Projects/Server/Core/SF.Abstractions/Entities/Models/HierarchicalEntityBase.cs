using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
namespace SF.Data.Models
{
	public abstract class ItemEntityBase<TKey, TContainerKey, TContainer> :
		ObjectEntityBase<TKey>,
		IItemEntity<TContainerKey>,
		IItemEntityOrder
		where TKey : IEquatable<TKey>
	{
		[Comment("容器ID")]
		public virtual TContainerKey ContainerId { get; set; }

		[TableVisible]
		[Ignore]
		[Comment("容器")]
		public virtual string ContainerName { get; set; }

		[Ignore]
		[Comment("容器")]
		public virtual TContainer Container { get; set; }

		[Comment("排位")]
		[TableVisible]
		[Optional]
		public virtual int ItemOrder { get; set; }

	}
	public abstract class ItemEntityBase<TContainer> : ItemEntityBase<long, long?, TContainer>
	{ }

	public abstract class UIItemEntityBase<TKey, TContainerKey, TContainer> :
		UIObjectEntityBase<TKey>,
		IItemEntity<TContainerKey>,
		IItemEntityOrder
		where TKey : IEquatable<TKey>
	{
		[Comment("容器ID")]
		public virtual TContainerKey ContainerId { get; set; }

		[TableVisible]
		[Ignore]
		[Comment("容器")]
		public virtual string ContainerName { get; set; }

		[Ignore]
		[Comment("容器")]
		public virtual TContainer Container { get; set; }

		[Comment("排位")]
		[TableVisible]
		[Optional]
		public virtual int ItemOrder { get; set; }

	}
	public abstract class UIItemEntityBase<TContainer> : 
		UIItemEntityBase<long, long?, TContainer>,
		IItemEntity
	{ }


	public abstract class ContainerEntityBase<TContainerKey, TItem, TItemContainerKey> :
		ObjectEntityBase<TContainerKey>,
		IContainerEntity<TContainerKey, TItem, TItemContainerKey>
		where TItem : IItemEntity<TItemContainerKey>
		where TContainerKey : IEquatable<TContainerKey>
	{
		public virtual IEnumerable<TItem> Items { get; set; }
	}
	public abstract class ContainerEntityBase<TItem> :
		ContainerEntityBase<long, TItem, long?>,
		IContainerEntity<TItem>
		where TItem : IItemEntity
	{ }
	public abstract class UIContainerEntityBase<TContainerKey, TItem, TItemContainerKey> :
		UIObjectEntityBase<TContainerKey>,
		IContainerEntity<TContainerKey, TItem, TItemContainerKey>
		where TItem : IItemEntity<TItemContainerKey>
		where TContainerKey : IEquatable<TContainerKey>
	{
		public virtual IEnumerable<TItem> Items { get; set; }
	}
	public abstract class UIContainerEntityBase<TItem> :
		UIContainerEntityBase<long, TItem, long?>,
		IContainerEntity<TItem>
		where TItem : IItemEntity
	{ }

	public abstract class TreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey> :
		ItemEntityBase<TTreeNodeKey,TTreeParentKey,TNodeEntity>,
		ITreeNodeEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TNodeEntity : TreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey>
	{
		[Ignore]
		public virtual IEnumerable<TNodeEntity> Children { get; set; }
	}
	public abstract class TreeNodeEntityBase<TNodeEntity> :
		TreeNodeEntityBase<TNodeEntity, long, long?>,
		ITreeNodeEntity<TNodeEntity>
		where TNodeEntity : TreeNodeEntityBase<TNodeEntity>
	{

	}
	public abstract class UITreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey> :
		UIItemEntityBase<TTreeNodeKey, TTreeParentKey, TNodeEntity>,
		ITreeNodeEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TNodeEntity : UITreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey>
	{
		[Comment("子项")]
		public virtual IEnumerable<TNodeEntity> Children { get; set; }
	}
	public abstract class UITreeNodeEntityBase<TNodeEntity> :
		UITreeNodeEntityBase<TNodeEntity, long, long?>,
		ITreeNodeEntity<TNodeEntity>
		where TNodeEntity : UITreeNodeEntityBase<TNodeEntity>
	{

	}

	public abstract class TreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey, TItemEntity, TItemContainerKey> :
		TreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey>,
		ITreeContainerEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey, TItemEntity, TItemContainerKey>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TNodeEntity : TreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey, TItemEntity, TItemContainerKey>
		where TItemEntity : IItemEntity<TItemContainerKey>
	{
		public virtual IEnumerable<TItemEntity> Items { get; set; }
	}

	public abstract class TreeContainerEntityBase<TTreeContainerEntity, TItemEntity> :
		TreeContainerEntityBase<TTreeContainerEntity, long, long?, TItemEntity, long?>,
		ITreeContainerEntity<TTreeContainerEntity, TItemEntity>
		where TTreeContainerEntity : TreeContainerEntityBase<TTreeContainerEntity, TItemEntity>
		where TItemEntity : IItemEntity
	{
	}

	public abstract class UITreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey, TItemEntity, TItemContainerKey> :
		UITreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey>,
		ITreeContainerEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey, TItemEntity, TItemContainerKey>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TNodeEntity : UITreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey, TItemEntity, TItemContainerKey>
		where TItemEntity : IItemEntity<TItemContainerKey>
	{
		[Comment("项目")]
		public virtual IEnumerable<TItemEntity> Items { get; set; }
	}

	public abstract class UITreeContainerEntityBase<TTreeContainerEntity, TItemEntity> :
		UITreeContainerEntityBase<TTreeContainerEntity, long, long?, TItemEntity, long?>,
		ITreeContainerEntity<TTreeContainerEntity, TItemEntity>
		where TTreeContainerEntity : UITreeContainerEntityBase<TTreeContainerEntity, TItemEntity>
		where TItemEntity : IItemEntity
	{
	}

}

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

using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Entities.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace SF.Entities.DataModels
{

	public abstract class ItemEntityBase<TKey, TContainerKey, TContainer> :
		ObjectEntityBase<TKey>,
		IItemEntity<TContainerKey>,
		IItemEntityOrder
		where TKey : IEquatable<TKey>
	{
		/// <summary>
		/// 容器
		/// </summary>
		[Index("container",Order =1)]
		public virtual TContainerKey ContainerId { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		[TableVisible]
		[Index("container", Order = 2)]
		public virtual int ItemOrder { get; set; }

		/// <summary>
		/// 容器
		/// </summary>
		[ForeignKey(nameof(ContainerId))]
		public virtual TContainer Container { get; set; }
	}
	public abstract class ItemEntityBase<TContainer> :
		ItemEntityBase<long, long?, TContainer>,
		IItemEntity
	{ }

	public abstract class UIItemEntityBase<TKey, TContainerKey, TContainer> :
		UIObjectEntityBase<TKey>,
		IItemEntity<TContainerKey>,
		IItemEntityOrder
		where TKey : IEquatable<TKey>
	{
		/// <summary>
		/// 容器
		/// </summary>
		[Index("container", Order = 1)]
		public virtual TContainerKey ContainerId { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		[TableVisible]
		[Index("container", Order = 2)]
		[ItemOrder]
		[Ignore]
		public virtual int ItemOrder { get; set; }

		/// <summary>
		/// 容器
		/// </summary>
		[ForeignKey(nameof(ContainerId))]
		public virtual TContainer Container { get; set; }
	}
	public abstract class UIItemEntityBase<TContainer> : 
		UIItemEntityBase<long, long?, TContainer>,
		IItemEntity
	{ }


	public abstract class ContainerEntityBase<TContainer,TContainerKey, TItem, TItemKey, TItemContainerKey> :
		ObjectEntityBase<TContainerKey>,
		IContainerEntity<TContainerKey, TItem, TItemContainerKey>
		where TItem : ItemEntityBase<TItemKey,TItemContainerKey,TContainer>
		where TItemKey: IEquatable<TItemKey>
		where TContainerKey : IEquatable<TContainerKey>
	{
		[InverseProperty(nameof(ItemEntityBase<TItemKey, TItemContainerKey, TContainer > .Container))]
		public virtual IEnumerable<TItem> Items { get; set; }
	}
	public abstract class ContainerEntityBase<TContainer,TItem> :
		ContainerEntityBase<TContainer,long, TItem, long, long?>,
		IContainerEntity<TItem>
		where TContainer: ContainerEntityBase<TContainer, TItem>
		where TItem : ItemEntityBase<TContainer>
	{ }
	public abstract class UIContainerEntityBase<TContainer, TContainerKey, TItem, TItemKey, TItemContainerKey> :
		UIObjectEntityBase<TContainerKey>,
		IContainerEntity<TContainerKey, TItem, TItemContainerKey>
		where TItem : ItemEntityBase<TItemKey, TItemContainerKey, TContainer>
		where TItemKey : IEquatable<TItemKey>
		where TContainerKey : IEquatable<TContainerKey>
	{
		[InverseProperty(nameof(ItemEntityBase<TItemKey, TItemContainerKey, TContainer>.Container))]
		public virtual IEnumerable<TItem> Items { get; set; }
	}
	public abstract class UIContainerEntityBase<TContainer, TItem> :
		UIContainerEntityBase<TContainer, long, TItem, long, long?>,
		IContainerEntity<TItem>
		where TContainer : ContainerEntityBase<TContainer, TItem>
		where TItem : ItemEntityBase<TContainer>
	{ }

	public abstract class TreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey> :
		ItemEntityBase<TTreeNodeKey, TTreeParentKey, TNodeEntity>,
		ITreeNodeEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TNodeEntity : TreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey>
	{
		[InverseProperty(nameof(Container))]
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
		[InverseProperty(nameof(Container))]
		public virtual IEnumerable<TNodeEntity> Children { get; set; }
	}
	public abstract class UITreeNodeEntityBase<TNodeEntity> :
		UITreeNodeEntityBase<TNodeEntity, long, long?>,
		ITreeNodeEntity<TNodeEntity>
		where TNodeEntity : UITreeNodeEntityBase<TNodeEntity>
	{

	}

	public abstract class TreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemEntityKey> :
		TreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey>,
		ITreeContainerEntity<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemContainerKey>
		where TNodeEntity : TreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemEntityKey>
		where TItemEntity : ItemEntityBase<TItemEntityKey,TItemContainerKey,TNodeEntity>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TItemEntityKey : IEquatable<TItemEntityKey>
	{
		[InverseProperty(nameof(ItemEntityBase<TItemEntityKey, TItemContainerKey, TNodeEntity>.Container))]
		public virtual IEnumerable<TItemEntity> Items { get; set; }
	}

	public abstract class TreeContainerEntityBase<TTreeContainerEntity, TItemEntity> :
		TreeContainerEntityBase<TTreeContainerEntity, long, long?, TItemEntity, long>,
		ITreeContainerEntity<TTreeContainerEntity, TItemEntity>
		where TTreeContainerEntity : TreeContainerEntityBase<TTreeContainerEntity, TItemEntity>
		where TItemEntity : ItemEntityBase<TTreeContainerEntity>
	{
	}
	public abstract class UITreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemEntityKey> :
		UITreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey>,
		ITreeContainerEntity<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemContainerKey>
		where TNodeEntity : UITreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemEntityKey>
		where TItemEntity : UIItemEntityBase<TItemEntityKey, TItemContainerKey, TNodeEntity>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TItemEntityKey : IEquatable<TItemEntityKey>
	{
		[InverseProperty(nameof(ItemEntityBase<TItemEntityKey, TItemContainerKey, TNodeEntity>.Container))]
		public virtual IEnumerable<TItemEntity> Items { get; set; }
	}

	public abstract class UITreeContainerEntityBase<TTreeContainerEntity, TItemEntity> :
		UITreeContainerEntityBase<TTreeContainerEntity, long, long?, TItemEntity, long>,
		ITreeContainerEntity<TTreeContainerEntity, TItemEntity>
		where TTreeContainerEntity : UITreeContainerEntityBase<TTreeContainerEntity, TItemEntity>
		where TItemEntity : UIItemEntityBase<TTreeContainerEntity>
	{
	}



}

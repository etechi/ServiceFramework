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
using SF.Sys.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Sys.Entities.DataModels
{

	public abstract class DataItemEntityBase<TKey, TContainerKey, TContainer> :
		DataObjectEntityBase<TKey>,
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
	public abstract class DataItemEntityBase<TContainer> :
		DataItemEntityBase<long, long?, TContainer>,
		IItemEntity
	{ }

	public abstract class DataUIItemEntityBase<TKey, TContainerKey, TContainer> :
		DataUIObjectEntityBase<TKey>,
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
	public abstract class DataUIItemEntityBase<TContainer> : 
		DataUIItemEntityBase<long, long?, TContainer>,
		IItemEntity
	{ }


	public abstract class DataContainerEntityBase<TContainer,TContainerKey, TItem, TItemKey, TItemContainerKey> :
		DataObjectEntityBase<TContainerKey>,
		IContainerEntity<TContainerKey, TItem, TItemContainerKey>
		where TItem : DataItemEntityBase<TItemKey,TItemContainerKey,TContainer>
		where TItemKey: IEquatable<TItemKey>
		where TContainerKey : IEquatable<TContainerKey>
	{
		[InverseProperty(nameof(DataItemEntityBase<TItemKey, TItemContainerKey, TContainer > .Container))]
		public virtual IEnumerable<TItem> Items { get; set; }
	}
	public abstract class DataContainerEntityBase<TContainer,TItem> :
		DataContainerEntityBase<TContainer,long, TItem, long, long?>,
		IContainerEntity<TItem>
		where TContainer: DataContainerEntityBase<TContainer, TItem>
		where TItem : DataItemEntityBase<TContainer>
	{ }
	public abstract class DataUIContainerEntityBase<TContainer, TContainerKey, TItem, TItemKey, TItemContainerKey> :
		DataUIObjectEntityBase<TContainerKey>,
		IContainerEntity<TContainerKey, TItem, TItemContainerKey>
		where TItem : DataUIItemEntityBase<TItemKey, TItemContainerKey, TContainer>
		where TItemKey : IEquatable<TItemKey>
		where TContainerKey : IEquatable<TContainerKey>
	{
		[InverseProperty(nameof(DataItemEntityBase<TItemKey, TItemContainerKey, TContainer>.Container))]
		public virtual IEnumerable<TItem> Items { get; set; }
	}
	public abstract class DataUIContainerEntityBase<TContainer, TItem> :
		DataUIContainerEntityBase<TContainer, long, TItem, long, long?>,
		IContainerEntity<TItem>
		where TContainer : DataUIContainerEntityBase<TContainer, TItem>
		where TItem : DataUIItemEntityBase<TContainer>
	{ }

	public abstract class DataTreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey> :
		DataItemEntityBase<TTreeNodeKey, TTreeParentKey, TNodeEntity>,
		ITreeNodeEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TNodeEntity : DataTreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey>
	{
		[InverseProperty(nameof(Container))]
		public virtual IEnumerable<TNodeEntity> Children { get; set; }
	}
	public abstract class DataTreeNodeEntityBase<TNodeEntity> :
		DataTreeNodeEntityBase<TNodeEntity, long, long?>,
		ITreeNodeEntity<TNodeEntity>
		where TNodeEntity : DataTreeNodeEntityBase<TNodeEntity>
	{

	}
	public abstract class DataUITreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey> :
		DataUIItemEntityBase<TTreeNodeKey, TTreeParentKey, TNodeEntity>,
		ITreeNodeEntity<TNodeEntity, TTreeNodeKey, TTreeParentKey>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TNodeEntity : DataUITreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TTreeParentKey>
	{
		[InverseProperty(nameof(Container))]
		public virtual IEnumerable<TNodeEntity> Children { get; set; }
	}
	public abstract class DataUITreeNodeEntityBase<TNodeEntity> :
		DataUITreeNodeEntityBase<TNodeEntity, long, long?>,
		ITreeNodeEntity<TNodeEntity>
		where TNodeEntity : DataUITreeNodeEntityBase<TNodeEntity>
	{

	}

	public abstract class DataTreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemEntityKey> :
		DataTreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey>,
		ITreeContainerEntity<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemContainerKey>
		where TNodeEntity : DataTreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemEntityKey>
		where TItemEntity : DataItemEntityBase<TItemEntityKey,TItemContainerKey,TNodeEntity>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TItemEntityKey : IEquatable<TItemEntityKey>
	{
		[InverseProperty(nameof(DataItemEntityBase<TItemEntityKey, TItemContainerKey, TNodeEntity>.Container))]
		public virtual IEnumerable<TItemEntity> Items { get; set; }
	}

	public abstract class DataTreeContainerEntityBase<TTreeContainerEntity, TItemEntity> :
		DataTreeContainerEntityBase<TTreeContainerEntity, long, long?, TItemEntity, long>,
		ITreeContainerEntity<TTreeContainerEntity, TItemEntity>
		where TTreeContainerEntity : DataTreeContainerEntityBase<TTreeContainerEntity, TItemEntity>
		where TItemEntity : DataItemEntityBase<TTreeContainerEntity>
	{
	}
	public abstract class DataUITreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemEntityKey> :
		DataUITreeNodeEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey>,
		ITreeContainerEntity<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemContainerKey>
		where TNodeEntity : DataUITreeContainerEntityBase<TNodeEntity, TTreeNodeKey, TItemContainerKey, TItemEntity, TItemEntityKey>
		where TItemEntity : DataUIItemEntityBase<TItemEntityKey, TItemContainerKey, TNodeEntity>
		where TTreeNodeKey : IEquatable<TTreeNodeKey>
		where TItemEntityKey : IEquatable<TItemEntityKey>
	{
		[InverseProperty(nameof(DataItemEntityBase<TItemEntityKey, TItemContainerKey, TNodeEntity>.Container))]
		public virtual IEnumerable<TItemEntity> Items { get; set; }
	}

	public abstract class DataUITreeContainerEntityBase<TTreeContainerEntity, TItemEntity> :
		DataUITreeContainerEntityBase<TTreeContainerEntity, long, long?, TItemEntity, long>,
		ITreeContainerEntity<TTreeContainerEntity, TItemEntity>
		where TTreeContainerEntity : DataUITreeContainerEntityBase<TTreeContainerEntity, TItemEntity>
		where TItemEntity : DataUIItemEntityBase<TTreeContainerEntity>
	{
	}



}

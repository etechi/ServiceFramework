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

using SF.Sys.Entities.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Entities
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
		/// <summary>
		/// 通过ID获取容器对象
		/// </summary>
		/// <param name="Key"></param>
		/// <returns></returns>
		Task<TContainer> LoadContainerAsync(TKey Key);
	}
	public interface IContainerItemsListable<TContainerKey, TItem>
	{
		/// <summary>
		/// 获取容器中的对象
		/// </summary>
		/// <param name="Container"></param>
		/// <param name="Paging"></param>
		/// <returns></returns>
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

	/// <summary>
	/// 获取容器中的子容器
	/// </summary>
	/// <typeparam name="TContainerKey"></typeparam>
	/// <typeparam name="TContainer"></typeparam>
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

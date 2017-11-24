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

using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.Entities.Annotations;
using SF.Sys.NetworkService;

namespace SF.Common.FrontEndContents.Friendly
{
	[EntityManager]
	[NetworkService]
	public interface IItemGroupManager<T> : 
		IEntityInstanceUpdater<ItemGroup<T>>
		where T:LinkItemBase
	{ }
	public interface IImageTextItemGroupManager:
		IItemGroupManager<ImageTextItem>
	{
	}

	public interface IImageItemGroupManager :
		IItemGroupManager<ImageItem>
	{
	}
	public interface ITextGroupTextItemGroupManager:
		IItemGroupManager<TextGroupItem<TextItem>>
	{
	}

	[EntityManager]
	[NetworkService]
	public interface IItemGroupListManager<T> :
		IEntityLoadable<ObjectKey<long>, ItemGroup<T>>,
		IEntityUpdator<ItemGroup<T>>,
		IEntityListable<ItemGroup<T>>
		where T : LinkItemBase
	{
	}


}

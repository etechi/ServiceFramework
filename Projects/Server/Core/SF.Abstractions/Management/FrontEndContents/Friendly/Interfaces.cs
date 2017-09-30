using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
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

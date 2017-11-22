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
using System;

namespace SF.Sys.Entities
{
	[Flags]
	public enum EntityManagerCapability
	{
		Creatable=1,
		Updatable=2,
		Deletable=4,
		All=7
	}
	public interface IEntityRemover<TKey >
	{
		Task RemoveAsync(TKey Key);
	}
	public interface IEntityAllRemover
	{
		Task RemoveAllAsync();
	}
	public interface IEntityManagerCapabilities 
	{
		EntityManagerCapability Capabilities { get; }
	}
	public interface IEntityEditableLoader<TKey, TEntity>
	{
		Task<TEntity> LoadForEdit(TKey Key);
	}
	public interface IEntityUpdator<TEntity>
	{
		Task UpdateAsync(TEntity Entity);
	}
	public interface IEntityCreator<TKey,TEntity>
	{
		Task<TKey> CreateAsync(TEntity Entity);
	}
	public interface IEntityManager<TKey, TEntity> :
		IEntityManagerCapabilities,
		IEntityRemover<TKey>,
		IEntityAllRemover,
		IEntityEditableLoader<TKey, TEntity>,
		IEntityCreator<TKey,TEntity>,
		IEntityUpdator<TEntity>
	{
		
		
	}
}

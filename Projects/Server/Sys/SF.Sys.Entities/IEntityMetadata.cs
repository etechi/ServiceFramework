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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Core.ServiceManagement;

namespace SF.Sys.Entities
{
	[Flags]
	public enum EntityCapability
	{
		None=0,
		Loadable=0x01,
		BatchLoadable=0x02,
		Queryable=0x04,
		Creatable=0x08,
		Updatable=0x10,
		EditableLoadable=0x20,
		Removable=0x40,
		AllRemovable=0x80
	}
	public interface IEntityMetadata
	{
		string Ident { get; }
		string Name { get; }
		string GroupIdent { get; }
		string GroupName { get; }
		string Description { get; }

		Type EntityKeyType { get; }
		Type EntityDetailType { get; }
		Type EntitySummaryType { get; }
		Type EntityEditableType { get; }
		Type EntityManagerType { get; }
		Type QueryArgumentType { get; }

		IEnumerable<Type> EntityTypes { get; }
		EntityCapability EntityManagerCapability { get; }
	}
	public interface IEntityMetadataCollection: IEnumerable<IEntityMetadata>
	{
		IEntityMetadata FindByEntityType(Type EntityEntityType);
		IEntityMetadata FindByManagerType(Type ManagerType);
		IEntityMetadata FindByTypeIdent(string Ident);
	}
}

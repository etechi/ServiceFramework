using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Core.ServiceManagement;

namespace SF.Entities
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
		string GroupName { get; }
		string Description { get; }
		Type EntityKeyType { get; }
		Type EntityDetailType { get; }
		Type EntitySummaryType { get; }
		Type EntityEditableType { get; }
		Type EntityManagerType { get; }
		Type QueryArgumentType { get; }
		EntityCapability EntityManagerCapability { get; }
	}
	public interface IEntityMetadataCollection: IEnumerable<IEntityMetadata>
	{
		IEntityMetadata FindByEntityType(Type EntityEntityType);
		IEntityMetadata FindByManagerType(Type ManagerType);
		IEntityMetadata FindByTypeIdent(string Ident);
	}
}

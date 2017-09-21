using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using SF.Metadata;
using SF.Core.ServiceManagement;
using System.Collections;

namespace SF.Entities
{
	class EntityMetadata : IEntityMetadata
	{
		public string Ident { get; set; }
		public string Name { get; set; }
		public string GroupName { get; set; }
		public string Description { get; set; }
		public Type EntityKeyType { get; set; }

		public Type EntityDetailType { get; set; }

		public Type EntitySummaryType { get; set; }
		public Type QueryArgumentType { get; set; }

		public Type EntityEditableType { get; set; }

		public Type EntityManagerType { get; set; }

		public EntityCapability EntityManagerCapability { get; set; }
	}
	class EntityMetadataCollection : IEntityMetadataCollection
	{
		IEntityMetadata[] Metadatas { get; }
		Dictionary<string, IEntityMetadata> IdentDict { get; }
		Dictionary<Type, IEntityMetadata> EntityDict { get; }
		Dictionary<Type, IEntityMetadata> ManagerDict { get; }

		static void Validate<T>(IEntityMetadata[] metas,Func<IEntityMetadata,T> key,string name)
		{
			var str = metas.GroupBy(key).Where(g => g.Count() > 1)
				.Select(g => g.Select(i => i.EntityManagerType).Join(",") + "的" + name + "重复，都为:" + g.Key)
				.Join(";");
			if (str.Length > 0)
				throw new ArgumentException(str);
		}
		void ValidateEntityType(Type type)
		{
			foreach (var p in type.AllPublicInstanceProperties())
			{
				var eia = p.GetCustomAttribute<EntityIdentAttribute>();
			}

		}
		void ValidateMetadata(IEntityMetadata meta)
		{
			if (meta.EntityDetailType != null)
				ValidateEntityType(meta.EntityDetailType);
			if(meta.EntityDetailType!=null)
				ValidateEntityType(meta.EntityDetailType);
			if (meta.EntityEditableType != null)
				ValidateEntityType(meta.EntityEditableType);
		}

		public EntityMetadataCollection(IEnumerable<IEntityMetadata> Metadatas)
		{
			this.Metadatas = Metadatas.ToArray();
			Validate(this.Metadatas, m => m.Ident, "标识");
			Validate(this.Metadatas, m => m.EntityDetailType, "实体类型");
			Validate(this.Metadatas, m => m.EntityManagerType, "管理类型");

			this.IdentDict = this.Metadatas.ToDictionary(d => d.Ident);
			this.ManagerDict = this.Metadatas.ToDictionary(d => d.EntityManagerType);
			this.EntityDict = this.Metadatas.ToDictionary(d => d.EntityDetailType);

			foreach (var m in this.Metadatas)
				ValidateMetadata(m);
		}

		public IEntityMetadata FindByDetailType(Type EntityDetailType)
		{
			return EntityDict.Get(EntityDetailType);
		}

		public IEntityMetadata FindByManagerType(Type ManagerType)
		{
			return ManagerDict.Get(ManagerType);
		}

		public IEntityMetadata FindByTypeIdent(string Ident)
		{
			return IdentDict.Get(Ident);
		}

		public IEnumerator<IEntityMetadata> GetEnumerator()
		{
			foreach (var i in Metadatas)
				yield return i;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

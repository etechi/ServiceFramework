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
using SF.ADT;

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
		void ValidateEntityType(Type type,Action<Type> entityValidater)
		{
			foreach (var p in type.AllPublicInstanceProperties())
			{
				var eia = p.GetCustomAttribute<EntityIdentAttribute>();
				if (eia != null && eia.EntityType != null)
					entityValidater(eia.EntityType);
			}

		}
		void ValidateMetadata(IEntityMetadata meta,Action<Type> entityValidator)
		{
			if (meta.EntityDetailType != null)
				ValidateEntityType(meta.EntityDetailType,entityValidator);
			if(meta.EntityDetailType!=null)
				ValidateEntityType(meta.EntityDetailType,entityValidator);
			if (meta.EntityEditableType != null)
				ValidateEntityType(meta.EntityEditableType,entityValidator);
		}

		Action<PropertyInfo,Type > GetEntityIndentValidator()
		{
			var allType = (from meta in Metadatas
						   let declareType =meta.EntityDetailType
						   from at in Link.ToEnumerable(declareType, t => t.BaseType)
						   select at).Distinct();
			var typeTrees = Tree.Build(
				allType, 
				t => t.BaseType==typeof(object) || t.BaseType==typeof(MarshalByRefObject)?null:t.BaseType
				);
			var typeNodeDict =ADT.Tree.AsEnumerable(typeTrees,n=>n).ToDictionary(n => n.Value);
			return (p, ot) =>
			{
				if (!typeNodeDict.TryGetValue(ot, out var n))
					throw new ArgumentException($"{p.DeclaringType}属性{p.Name}找不到实体{ot}");
				while(n.Count>0)
				{
					if(n.Count>1)
						throw new ArgumentException($"{p.DeclaringType}属性{p.Name}的实体{ot}或子类{n.Value}有多个子类{n.Select(c=>c.Value).Join(",")}");
					n = n[0];
				}
			};
		}
		public EntityMetadataCollection(IEnumerable<IEntityMetadata> Metadatas)
		{
			this.Metadatas = Metadatas.ToArray();
			Validate(this.Metadatas, m => m.Ident, "标识");
			Validate(this.Metadatas, m => m.EntityDetailType, "实体类型");
			Validate(this.Metadatas, m => m.EntityManagerType, "管理类型");

			this.IdentDict = this.Metadatas.ToDictionary(d => d.Ident);
			this.ManagerDict = this.Metadatas.ToDictionary(d => d.EntityManagerType);
			this.EntityDict =
				(from m in this.Metadatas
				from t in (
					from ct in EnumerableEx.From(m.EntityDetailType,m.EntityEditableType,m.EntitySummaryType)
					where ct!=null
					from t in ADT.Link.ToEnumerable(ct, it => it.BaseType)
					select t
					).Distinct()
				group (t,m) by t into g
				where g.Count()==1
				select g.First()
				)
				.ToDictionary(i=>i.t,i=>i.m);


			var validator = GetEntityIndentValidator();
			
			(from m in this.Metadatas
			 from type in new[] { m.EntityDetailType, m.EntityEditableType, m.EntitySummaryType, m.QueryArgumentType }
			 where type != null
			 from prop in type.AllPublicInstanceProperties()
			 let ea = prop.GetCustomAttribute<EntityIdentAttribute>()
			 where ea != null && ea.EntityType != null
			 select (prop, ea.EntityType)
			 ).ForEach(p =>
				validator(
					p.prop, p.EntityType
					)
				);
		}

		public IEntityMetadata FindByEntityType(Type EntityDetailType)
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

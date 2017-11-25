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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using SF.Sys.Reflection;
using SF.Sys.Services;
using SF.Sys.Collections.Generic;
using SF.Sys.Linq;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders
{

	public class BaseDataModel
	{

	}
	
	public class DataModelTypeCollection : Dictionary<string, Type>, IDataModelTypeCollection
	{

	}

	public class DataModelBuilder : IDataModelBuildContext
	{

		IDynamicTypeBuilder DynamicTypeBuilder { get; }

		public Dictionary<string, TypeExpression> TypeExpressions { get; } = new Dictionary<string, TypeExpression>();

		public IMetadataCollection Metadata { get; }
		public IEnumerable<IEntityType> EntityTypes { get; }
		IDataModelBuildProvider[] DataModelBuildProviders { get; }
		NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver { get; }

		//Dictionary<string, TypeExpression> IDataModelBuildContext.TypeExpressions => throw new NotImplementedException();

		public DataModelBuilder(
			IMetadataCollection Metadata,
			IDynamicTypeBuilder DynamicTypeBuilder,
			IEnumerable<IEntityAutoCapability> AutoCapabilities,
			IEnumerable<IDataModelBuildProvider> DataModelBuildProviders,
			NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver
			)
		{
			this.DynamicTypeBuilder = DynamicTypeBuilder;
			this.Metadata = Metadata;
			this.EntityTypes = AutoCapabilities
				.Where(c => c.Capability.HasFlag(AutoCapability.GenerateDataModel))
				.Select(c => Metadata.EntityTypes.Get(c.EntityIdent) ?? throw new ArgumentException($"找不到自动生成数据对象时使用的实体类型{c.EntityIdent}"))
				.ToArray();

			this.DataModelBuildProviders = DataModelBuildProviders.OrderBy(p=>p.Priority).ToArray();
			this.DataModelAttributeGeneratorResolver = DataModelAttributeGeneratorResolver;
		}
		
		static volatile int TypeIdSeed = 1;
		static int NextTypeId()
		{
			return System.Threading.Interlocked.Increment(ref TypeIdSeed);
		}

		public IDataModelTypeCollection Build(string Prefix)
		{
			//throw new ArgumentException(metas.ToString());
			foreach (var et in EntityTypes)
			{
				var type = new TypeExpression(
					et.Name + "_" + NextTypeId(),
					new SystemTypeReference(typeof(BaseDataModel)),
					TypeAttributes.Public
					);

				foreach (var a in et.Attributes)
				{
					var g = DataModelAttributeGeneratorResolver(a.Name);
					if (g == null)
						continue;
						//throw new ArgumentException($"不支持生成特性{a.Name}的数据类特性，实体类型:{typeExpr.Name}");
					var aa = g.Generate(a);
					if (aa != null)
						type.CustomAttributes.Add(aa);
				}
				TypeExpressions.Add(et.Name, type);

			}

			foreach (var p in DataModelBuildProviders)
				p.BeforeBuildModel(this);

			foreach (var p in DataModelBuildProviders.Reverse())
				p.AfterBuildModel(this);

			var exprs = (from entityType in EntityTypes
						 let typeExpr = TypeExpressions.Get(entityType.Name)
						 where typeExpr!=null
						 select (entityType.Name, typeExpr)
						).ToArray();

			var re = new DataModelTypeCollection();
			var types = DynamicTypeBuilder.Build(exprs.Select(e => e.typeExpr));
			exprs
				.Zip(types, (x, type) => (x.Name,type))
				.ForEach(p=>re.Add(p.Name, p.type));
			return re;
		}
	}
}

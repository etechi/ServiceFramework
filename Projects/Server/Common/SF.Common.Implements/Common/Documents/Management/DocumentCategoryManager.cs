﻿#region Apache License Version 2.0
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

using SF.Core;
using SF.Core.CallPlans;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Metadata;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SF.Common.Documents.DataModels;

namespace SF.Common.Documents.Management
{
	public class DocumentCategoryManager :
		DocumentCategoryManager<CategoryInternal>,
		IDocumentCategoryManager
	{
		public DocumentCategoryManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
	}
	public class DocumentCategoryManager<TCategoryInternal> :
		AutoModifiableEntityManager<ObjectKey<long>, TCategoryInternal, TCategoryInternal, DocumentCategoryQueryArgument, TCategoryInternal, DataModels.DocumentCategory>,
		IDocumentCategoryManager<TCategoryInternal>
		where TCategoryInternal : CategoryInternal
	{
		public DocumentCategoryManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
	}

}

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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using SF.Sys.Data;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Internal;
using Remotion.Linq.Parsing.Structure;
using Microsoft.EntityFrameworkCore.Query;
using System.Threading;
using System.Data.SqlClient;

namespace SF.Sys.Data.EntityFrameworkCore
{

	//public class ServiceProtocolDataIndexAttributeConvention : AttributeToColumnAnnotationConvention<ServiceProtocol.Entities.IndexAttribute, IndexAnnotation>
	//{
	//	public ServiceProtocolDataIndexAttributeConvention() :
	//		base("Index",
	//			(p, a) => new IndexAnnotation(
	//				a.Select(i =>
	//					i.Name==null?
	//					new System.ComponentModel.DataAnnotations.Schema.IndexAttribute
	//					{
	//						IsClustered = i.IsClustered,
	//						IsUnique = i.IsUnique,
	//						Order = i.Order
	//					}:new System.ComponentModel.DataAnnotations.Schema.IndexAttribute(i.Name)
	//					  {
	//						  IsClustered=i.IsClustered,
	//						  IsUnique=i.IsUnique,
	//						  Order=i.Order
	//					  }
	//					)
	//			)
	//			)
	//	{ }
	//}

	
	public class EFException
    {
		//string FormatEntityValidationException(Microsoft.EntityFrameworkCore ValidationException e)
		//{
		//	return string.Join(";",
		//				from eve in e.EntityValidationErrors
		//				let type_name = eve.Entry.Entity.GetType().FullName
		//				from ve in eve.ValidationErrors
		//				select type_name + "." + ve.PropertyName + ":" + ve.ErrorMessage
		//			);
		//}
		//string FormatDbUpdateException(System.Data.Entity.Infrastructure.DbUpdateException e)
		//{
		//	return (from ee in e.Entries
		//			let type = ee.Entity.GetType()
		//			let ctn = ee.Entity.ToString()
		//			select type.FullName + ":" + ctn
		//			).Join(";");
		//}
		static string FormatDbUpdateConcurrencyException(Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException e)
		{
			//var ie = e.InnerException as System.Data.Entity.Core.OptimisticConcurrencyException;
			//if (ie != null)
			//	return (from ee in ie.StateEntries
			//			let type = ee.Entity.GetType()
			//			let id = ee.EntityKey.EntityKeyValues.Select(kv => kv.Key + "=" + kv.Value).Join(",")
			//			let ctn = ee.Entity.ToString()
			//			select type.FullName + ":" + id + ":" + ctn
			//		).Join(";");

			return (from ee in e.Entries
					let type = ee.Entity.GetType()
					let ctn = Json.Stringify(ee.Entity)
					select type.FullName + ":" + ctn
					).Join(";");
		}
		public static Exception MapException(System.Exception e)
		{
			var re = e.GetRootException();
			if(e is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException uce)
			{
				return new DbUpdateConcurrencyException(
					"数据并发更新错误：" + FormatDbUpdateConcurrencyException(uce),
					uce
					);
			}
			if (re is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException uce1)
			{
				return new DbUpdateConcurrencyException(
					"数据并发更新错误：" + FormatDbUpdateConcurrencyException(uce1),
					uce1
					);
			}
			if(re is SqlException se)
			{
				if (se.Number == 1205)
					return new DbDeadLockException("数据库事务发生死锁", se);
				else if (se.Number == 2601)
					throw new DbDuplicatedKeyException("键值冲突", e);
			}
			//var ve = e as DbEntityValidationException;
			//if(ve!=null)
			//	return new DbValidateException(
			//		"数据实体验证失败：" + FormatEntityValidationException(ve),
			//		ve
			//		);
			//var ce = e as System.Data.Entity.Infrastructure.DbUpdateConcurrencyException;
			//if (ce != null)
			//	return new DbUpdateConcurrencyException(
			//		"数据并发更新错误：" + FormatDbUpdateConcurrencyException(ce),
			//		ce);

			//var ue = e as System.Data.Entity.Infrastructure.DbUpdateException;
			//if (ue != null)
			//{
			//	if(ue.Entries.All(s=>s.State==EntityState.Added))
			//		return new DbDuplicatedKeyException(
			//			"主键或约束冲突：" + e.GetInnerExceptionMessage() + "：" + FormatDbUpdateException(ue),
			//			ue);
			//	else
			//		return new DbUpdateException(
			//			"数据更新失败：" + e.GetInnerExceptionMessage() + "：" + FormatDbUpdateException(ue),
			//			ue);
			//}


			//return new DbException("数据操作失败：" + e.GetInnerExceptionMessage(),e);
			return e;
		}
	
	}
	
}

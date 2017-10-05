﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SF.Core.ServiceManagement;
using SF.Entities.AutoEntityProvider.Internals;

namespace SF.Entities.AutoEntityProvider
{
	public static class AutoEntityProviderDIExtension
	{
		public static IServiceCollection AddAutoEntityType(
			this IServiceCollection sc,
			string Namespace,
			params Type[] Types
			)
			=> sc.AddAutoEntityType(Namespace, true, Types);

		public static IServiceCollection AddAutoEntityType(
			this IServiceCollection sc, 
			string Namespace, 
			bool AutoGenerateDataModel,
			params Type[] Types
			)
		{
			Types.ForEach(t =>
				sc.AddSingleton(sp => new AutoEntityType (Namespace,t, AutoGenerateDataModel))
				);
			return sc;
		}
	}


}
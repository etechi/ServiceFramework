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

namespace SF.Sys.Services
{
	public static class DataEntityServiceCollectionExtension
	{
		public static void AddDataModules(this IServiceCollection sc,string Prefix,params Type[] Types)
		{
			sc.AddSingleton(new Data.EntityDataModels(Types, Prefix));
		}
		public static void AddDataModules<T1>(this IServiceCollection sc,string Prefix=null) 
			where T1 : class 
			=> sc.AddDataModules(Prefix,typeof(T1));

		public static void AddDataModules<T1, T2>(this IServiceCollection sc, string Prefix = null) 
			where T1 : class
			where T2 : class
			=> sc.AddDataModules(Prefix, typeof(T1), typeof(T2));

		public static void AddDataModules<T1, T2,T3>(this IServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			=> sc.AddDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3));

		public static void AddDataModules<T1, T2, T3,T4>(this IServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3: class
			where T4: class
			=> sc.AddDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4));


		public static void AddDataModules<T1, T2, T3, T4,T5>(this IServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			=> sc.AddDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));


		public static void AddDataModules<T1, T2, T3, T4, T5,T6>(this IServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			=> sc.AddDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

		public static void AddDataModules<T1, T2, T3, T4, T5, T6,T7>(this IServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			=> sc.AddDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

		public static void AddDataModules<T1, T2, T3, T4, T5, T6, T7, T8>(this IServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			=> sc.AddDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
	}
}
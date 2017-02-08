using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.DI
{
	public static class DataEntityServiceCollectionExtension
	{
		public static void UseDataModules(this IDIServiceCollection sc,string Prefix,params Type[] Types)
		{
			sc.AddSingleton(new Data.Storage.EntityModels(Types, Prefix));
		}
		public static void UseDataModules<T1>(this IDIServiceCollection sc,string Prefix=null) 
			where T1 : class 
			=> sc.UseDataModules(Prefix,typeof(T1));

		public static void UseDataModules<T1, T2>(this IDIServiceCollection sc, string Prefix = null) 
			where T1 : class
			where T2 : class
			=> sc.UseDataModules(Prefix, typeof(T1), typeof(T2));

		public static void UseDataModules<T1, T2,T3>(this IDIServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			=> sc.UseDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3));

		public static void UseDataModules<T1, T2, T3,T4>(this IDIServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3: class
			where T4: class
			=> sc.UseDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4));


		public static void UseDataModules<T1, T2, T3, T4,T5>(this IDIServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			=> sc.UseDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));


		public static void UseDataModules<T1, T2, T3, T4, T5,T6>(this IDIServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			=> sc.UseDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

		public static void UseDataModules<T1, T2, T3, T4, T5, T6,T7>(this IDIServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			=> sc.UseDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

		public static void UseDataModules<T1, T2, T3, T4, T5, T6, T7, T8>(this IDIServiceCollection sc, string Prefix = null)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			=> sc.UseDataModules(Prefix, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
	}
}
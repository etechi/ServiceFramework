//#region Apache License Version 2.0
///*----------------------------------------------------------------
//Copyright 2017 Yang Chen (cy2000@gmail.com)

//Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
//except in compliance with the License. You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing, software distributed under the
//License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
//either express or implied. See the License for the specific language governing permissions
//and limitations under the License.
//Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
//----------------------------------------------------------------*/
//#endregion Apache License Version 2.0

//using System;
//using System.Reflection;
//using System.Linq.Expressions;
//using System.Collections.Generic;

//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using SF.Data;
//using Xunit;
//using SF.Applications;
//using SF.Core.Hosting;
//using SF.Core.ServiceManagement;
//using SF.Core.ServiceFeatures;
//using System.Threading.Tasks;
//using static SF.UT.Goals;
//using System.Linq;

//namespace SF.UT
//{
//	public class Goals
//	{
//		public class Target
//		{
//			protected void Expect(Target Target) { }
//			public static Target operator & (Target x,Target y)
//			{
//				return null;
//			}
//			public static Target operator | (Target x, Target y)
//			{
//				return null;
//			}
//		}
		

//		public class NewObjectTarget : Target { }



//		public class GlobalGoals : Target { }
//		public class MoreUsers : Target { }
//		public class MoreTrades : Target { }


//		public class MoreProducts : Target {

//		}
//		public class AddProduct : NewObjectTarget { }
//		public class PresentationProduct : Target { }

//		//public class 

//		public static  Target T<X>() {
//			var re = new Target[1];
//			return  re[0];
//		}
//		public static ref Target Let<X>()
//		{
//			var re = new Target[1];
//			return ref re[0];

//		}
//		public static ref Target Define<X>(Target Target)
//		{
//			var re = new Target[1];
//			return ref re[0];
//		}
//		public static ref Target More<X>()
//		{
//			var re = new Target[1];
//			return ref re[0];
//		}
//		public static Target WebApi<X>() => null;
//		public static Target Assert<X>(Func<X,bool> cond)
//		{
//			return null;
//		}
//		public class NewUser : Target { }
//		public class UserRegisterApi : Target { }


//		public class User
//		{
//			public int TradeCount { get; }
//		}
//		public class Entity<T> :Target 
//		{
//		}
//		public class New<T> : Target
//		{

//		}
//		public class NewTrade { }
//		public interface IUserService { }

//		public class TradeCompleted { }
//		public class TradeSuccess { }
//		public class TradeCancelled { }
//		public class GoodDelived { }
//		public void Test()
//		{
//			ITargetSystemBuilder b = null;

//			Let<GlobalGoals>() = T<MoreUsers>() & T<MoreTrades>();

//			Let<MoreUsers>() = More<NewUser>();
//			Let<NewUser>() = T<Entity<User>>();


//			Let<NewUser>() = 
//				WebApi<IUserService>() &
//				T<ServeUserRegister>();

//			Let<UserRegisterApi>() =

//			Let<MoreTrades>() = More<NewTrade>() & T<TradeCompleted>();
//			Let<NewTrade>() = More<NewTrade>();

//			Let<TradeCompleted>() = T<TradeSuccess>() | T<TradeCancelled>();
//			Let<TradeSuccess>() = T<GoodDelived>();

//			&Assert<User>(u=>u.TradeCount>10);


//			b.Expect<MoreUsers>().To((MoreUsers mu, NewUser mt) => mu & mt);
//			b.Expect<MoreUsers>().To((MoreUsers mu, NewUser mt) => mu & mt);

//			//Define((MoreUsers mu, MoreTrades mt) => mu & mt);
//		}

//	}
//	public interface ITarget {
//	}
//	public interface ITargetCollection
//	{
//		IQueryable<Target> Target<T>() where T:Target;
//	}
//	public interface IDefination
//	{
//		IQueryable<X> To<X, Y, Z>(Func<X, Y, Z> ts)
//			where X : Target
//			where Y : Target
//			where Z : Target
//			;
//	}
	
//	public interface ITargetSystemBuilder
//	{
//		ref Target T<X>() where X : Target;
//		IDefination Expect<T>();
//	}
//	public static class TargetQueryable
//	{
//		//public static IQueryable<R> SelectMany<T, X, R>(
//		//	this IQueryable<T> q,
//		//	Func<T, IEnumerable<X>> f1,
//		//	Func<T, X, R> f2
//		//	) where T: Target
//		//	where R: Target
//		//{
//		//	return null;
//		//}
//	}
//	public class GoalsTest
//	{
//		public void Test()
//		{

//			ITargetSystemBuilder b=null;
//			b.Expect<GlobalGoals>().To(
//				(q,ts) =>
//				from x in ts.Target<MoreUsers>()
//				from y in ts.Target<MoreTrades>()
//				select x & y
//				);
//			b[b.Target<GlobalGoal>] = MoreUsers & MoreTrades;

//		}

//	}

//}

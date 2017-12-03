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

using Xunit;
using System.Threading.Tasks;
using System;
using System.Linq;
using SF.Biz.MemberInvitations;
using SF.Sys.Tests;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Biz.MemberInvitations.Models;
using SF.Sys;
using SF.Sys.Entities;
using SF.Sys.ServiceFeatures;
//using SF.Sys.Entities.
namespace SF.UT.Promotions.Invitations
{
	public class InvitationTests : TestBase
	{
		public static async Task<T> InvitationTest<T>(IServiceProvider sp, Func<IMemberInvitationManagementService, Task<T>> Action)
		{
			return await sp.TestService(
				sim => sim.NewMemberInvitationServive(),
				Action
				);
		}
		

		

	

		interface IEntityValueTestProvider<T>
		{
			T NewSample();
			void EqualAssert(T x, T y);
		}

		class TestHelper
		{
			/*
			 * Create N
			 * 
			 * Remove 1 
			 * 
			 * Remove 1
			 * 
			 * 
			 * 
			 * 编辑1
			 * 编辑2
			 * 
			 * 查询
			 * 
			 * 删除ALL
			 * 
			 * 
			 * 
			 * 
			 * C: E->M
			 * M->
			 * 
			 */
			public static T Create<T>()
			{
				return default(T);
			}
		}
		//async Task BasicTest<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager>(
		//	TManager svc,
		//	IEntityManagerTestor<TDetail, TSummary, TEditable, TQueryArgument> testProvider,
		//	int createCount,
		//	ITestValueGenerator TestValueGenerator
		//	)
		//	where TKey : new()
		//	where TDetail : new()
		//	where TEditable : new()
		//	where TManager :
		//		IEntityLoadable<TKey, TDetail>,
		//		IEntityBatchLoadable<TKey, TDetail>,
		//		IEntityCreator<TKey, TEditable>,
		//		IEntityEditableLoader<TKey, TEditable>,
		//		IEntityRemover<TKey>,
		//		IEntityAllRemover,
		//		IEntityUpdator<TEditable>,
		//		IEntityQueryable<TSummary, TQueryArgument>,
		//		IEntityIdentQueryable<TKey, TQueryArgument>
		//{

		//	var createArguments = Enumerable.Range(0, createCount).Select(i => testProvider.Touch(new TEditable(), TestValueGenerator)).ToArray();
		//	var createIdents = new TKey[createCount];

		//	//创建对象
		//	for (var i = 0; i < createCount; i++)
		//		createIdents[i] = await svc.CreateAsync(createArguments[i]);


		//	//检查新创建的对象主键各不相同
		//	var distincted = createIdents.Distinct(Poco.DeepEqualityComparer<TKey>()).ToArray();
		//	Assert.Equal(distincted.Length, createCount);

		//	var emptyIdent = new TKey();
		//	//检查新创建的对象主键不为空
		//	foreach (var id in createIdents)
		//		Assert.True(!Poco.DeepEquals(id,emptyIdent));

		//	//检查新建结果
		//	var createResults = new TEditable[createCount];
		//	for (var i = 0; i < createCount; i++)
		//	{
		//		createResults[i] = await svc.LoadForEdit(createIdents[i]);
		//		testProvider.ValidateCreateResult(
		//			createArguments[i],
		//			createResults[i]
		//			);
		//	}



		//	//检查实体获取
		//	var details = new TDetail[createCount];
		//	for (var i = 0; i < createCount; i++)
		//	{
		//		details[i] = await svc.GetAsync(createIdents[i]);
		//		testProvider.ValidateDetail(createResults[i], details[i]);
		//	}

		//	//检查批量实体获取
		//	var batchDetails = await svc.GetAsync(createIdents);
		//	Assert.True(Poco.DeepEquals(details, batchDetails));


		//	var querySeedSummaries = details.Select(d => testProvider.ConvertToSummary(d)).ToArray();
		//	//检查查询
		//	foreach(var qa in testProvider.GenerateQueryArgument(querySeedSummaries))
		//	{
		//		var re=await svc.QueryAsync(qa.QueryArgument, qa.Paging);
		//		Assert.True(Poco.DeepEquals(re.Items.ToArray(), qa.Results));

		//		var ire = await svc.QueryIdentsAsync(qa.QueryArgument, qa.Paging);
		//		var eids = qa.Results.Select(i => Entity<TSummary>.GetKey<TKey>(i)).ToArray();
		//		Assert.True(Poco.DeepEquals(ire.Items.ToArray(),eids));
		//	}


		//	//检查实体更新
		//	var updateEditables = new TEditable[createCount];
		//	var updateResults = new TEditable[createCount];
		//	for (var i = 0; i < createCount; i++)
		//	{
		//		updateEditables[i] = testProvider.Touch(createResults[i], TestValueGenerator);
		//		await svc.UpdateAsync(updateEditables[i]);
		//		updateResults[i] = await svc.LoadForEdit(createIdents[i]);
		//		testProvider.ValidateUpdateResult(updateEditables[i], updateResults[i]);
		//	}

		//	//删除部分
		//	var removeIds = createIdents.Where((ci, i) => i % 2 == 0).ToArray();
		//	var restIds = createIdents.Where((ci, i) => i % 2 != 0).ToArray();
		//	foreach (var rid  in removeIds)
		//	{
		//		await svc.RemoveAsync(rid);
		//		Assert.Equal(default(TDetail),await svc.GetAsync(rid));
		//	}

		//	//删除剩余部分
		//	await svc.RemoveAllAsync();
		//	foreach(var rid in restIds)
		//		Assert.Equal(default(TDetail), await svc.GetAsync(rid));
		//}


		//public interface IValueTestor
		//{

		//}
		//public interface IValueTestor<T> : IValueTestor
		//{
		//	T NewSample(ITestValueGenerator ValueGenerator);
		//	T Touch(T Value);
		//	bool CreateCheck(T CreateArgument,T Editable);
		//}
		//public interface IValueTestorProvider
		//{
		//	IValueTestor GetValueTestor(Type Type, PropertyInfo Property);
		//}

		//class MemberInvitationTestProvider :
		//	IEntityManagerTestProvider<
		//		MemberInvitationInternal,
		//		MemberInvitationInternal,
		//		MemberInvitationInternal,
		//		MemberInvitationQueryArgument
		//		>
		//{
		//	public MemberInvitationInternal ConvertToSummary(MemberInvitationInternal Detail)
		//	{
		//		return Detail;
		//	}

		//	public void ValidateCreateResult(MemberInvitationInternal CreateArgument, MemberInvitationInternal LoadEditableResult)
		//	{
		//		CreateArgument.Id = LoadEditableResult.Id;
		//		Assert.True(Poco.DeepEquals(CreateArgument, LoadEditableResult));
		//	}

		//	public void ValidateSummary(MemberInvitationInternal Detail, MemberInvitationInternal Summary)
		//	{
		//		Assert.True(Poco.DeepEquals(Detail, Summary));
		//	}

		//	public void ValidateDetail(MemberInvitationInternal LoadEditableResult, MemberInvitationInternal Detail)
		//	{
		//		Assert.True(Poco.DeepEquals(Detail, LoadEditableResult));
		//	}

		//	public IEnumerable<QueryTestCase<MemberInvitationQueryArgument, MemberInvitationInternal>> GenerateQueryArgument(IEnumerable<MemberInvitationInternal> Summaries)
		//	{
		//		return Summaries.Select(s =>
		//			new QueryTestCase<MemberInvitationQueryArgument, MemberInvitationInternal>
		//			{
		//				QueryArgument = new MemberInvitationQueryArgument { Id = new ObjectKey<long> { Id = s.Id } },
		//				Paging = Paging.Single,
		//				Results = new[] { s }
		//			}
		//			);
		//	}

		//	static int _TestValueSeed=1;
		//	static int NextTestValueSeed()
		//	{
		//		return System.Threading.Interlocked.Increment(ref _TestValueSeed);
		//	}
		//	public MemberInvitationInternal NewCreateArgument()
		//	{
		//		return new MemberInvitationInternal
		//		{
		//			InvitorId = NextTestValueSeed(),
		//			Invitors = new long[] { NextTestValueSeed() },
		//			Time = new DateTime(2001, 1, 1).AddDays(NextTestValueSeed()),
		//			UserId = NextTestValueSeed()
		//		};
		//	}

		//	public MemberInvitationInternal Touch(MemberInvitationInternal editable)
		//	{
		//		return new MemberInvitationInternal
		//		{
		//			Id = editable.Id,
		//			InvitorId = editable.InvitorId + 1,
		//			Invitors = new[] { editable.Invitors[0] + 1 },
		//			UserId = editable.UserId + 1,
		//			Time = editable.Time.AddDays(1)
		//		};
		//	}

		//	public void ValidateUpdateResult(MemberInvitationInternal UpdateArgument, MemberInvitationInternal LoadEditableResult)
		//	{
		//		Assert.True(Poco.DeepEquals(UpdateArgument, LoadEditableResult));
		//	}
		//}
		//[Fact]
		//public async Task 创建_成功_New()
		//{
		//	await Scope(async (IServiceProvider sp) =>
		//		await InvitationTest(sp,
		//			async (svc) =>
		//			{
		//				await BasicTest<
		//					ObjectKey<long>, 
		//					MemberInvitationInternal, 
		//					MemberInvitationInternal, 
		//					MemberInvitationInternal,
		//					MemberInvitationQueryArgument,
		//					IMemberInvitationManagementService
		//					>(
		//					svc, 
		//					new MemberInvitationTestProvider(),
		//					100
		//					);
		//				return 0;
		//			}
		//			)
		//		);
		//}
		//[Fact]
		//public async Task 创建_成功_性能()
		//{
		//	await Scope(async (IServiceProvider sp) =>
		//		await InvitationTest(sp,
		//			async (svc) =>
		//			{
		//				await Task.WhenAll(
		//					Enumerable.Range(0, 100).Select(i =>
		//						sp.WithScope<IMemberInvitationManagementService, int>(
		//							async (isvc) =>
		//							{
		//								await BasicTest<
		//								 ObjectKey<long>,
		//								 MemberInvitationInternal,
		//								 MemberInvitationInternal,
		//								 MemberInvitationInternal,
		//								 MemberInvitationQueryArgument,
		//								 IMemberInvitationManagementService
		//								 >(
		//								 isvc,
		//								 new MemberInvitationTestProvider(),
		//								 10
		//								 );
		//								return 0;
		//							}
		//					)
		//				)
		//			);
		//				return 0;
		//			})
		//	);
		//}
		[Fact]
		public async Task 创建_成功1()
		{
			await Scope(async (IServiceProvider sp) =>
				await InvitationTest(sp,
					async (svc) =>
					{
						await sp.InitServices("test");
						return 0;
					}
				)
				);
		}
		[Fact]
		public async Task 创建_成功()
		{
			await Scope(async (IServiceProvider sp) =>
				await InvitationTest(sp,
					async (svc) =>
					{
						var dst = new MemberInvitationInternal
						{
							Time = DateTime.Now,
							UserId = 20,
							InvitorId = 20,
							Invitors = new long[] { 20 }
						};

						var rid = await svc.CreateAsync(dst);
						var nrid = await svc.CreateAsync(dst);

						Assert.NotEqual(dst.Id, rid.Id);
						Assert.NotEqual(nrid.Id, rid.Id);
						await svc.RemoveAsync(nrid);
						dst.Id = rid.Id;

						var o = await svc.GetAsync(rid);
						Assert.True(Poco.DeepEquals(dst, o));
						//Assert.Equal(dst, o);


						var oss = await svc.BatchGetAsync(new[] { rid });
						Assert.True(Poco.DeepEquals(new[] { dst }, oss));
						//Assert.Equal(dst, oss[0]);

						var ids = await svc.QueryIdentsAsync(new MemberInvitationQueryArgument() { Id = rid }, Paging.All);
						Assert.True(Poco.DeepEquals(new[] { rid }, ids.Items.ToArray()));


						var os = await svc.QueryAsync(new MemberInvitationQueryArgument() { Id = rid }, Paging.All);
						Assert.True(Poco.DeepEquals(new[] { dst }, os.Items.ToArray()));
						//Assert.Equal(dst, o);

						var e = await svc.LoadForEdit(rid);
						Assert.True(Poco.DeepEquals(dst, e));
						//Assert.Equal(dst, e);
						e.Invitors[0] += 1;
						e.InvitorId += 1;
						e.UserId += 1;
						var newTime = dst.Time.AddDays(1);
						e.Time = newTime;
						await svc.UpdateAsync(e);

						o = await svc.GetAsync(rid);
						//Assert.Equal(o, e);
						Assert.True(Poco.DeepEquals(o, e));

						await svc.RemoveAsync(rid);
						Assert.Null(await svc.GetAsync(rid));

						return rid;

					}
				)
			);
		}
		
	}


}

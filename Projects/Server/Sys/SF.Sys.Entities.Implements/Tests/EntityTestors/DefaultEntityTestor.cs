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
using System.Threading.Tasks;
using System.Reflection;
using SF.Sys.Entities.AutoTest;
using SF.Sys.Reflection;
using SF.Sys.Tests;
using SF.Sys.Services;
using SF.Sys.Logging;

namespace SF.Sys.Entities.Tests.EntityTestors
{
	public abstract class EntityTestExecutor
	{
		public abstract Task Test(ITestContext testContext);
	}
	public class EntityTestExecutor<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager>:
		EntityTestExecutor
		where TManager :
			IEntitySource<TKey, TSummary, TDetail, TQueryArgument>,
			IEntityManager<TKey, TEditable>
		where TKey : new()
		where TDetail : new()
		where TEditable : new()
		where TQueryArgument : IPagingArgument
	{

		public override async Task Test(ITestContext testContext)
		{
			var ctx = (IEntityTestContext<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager>)testContext;

			var svc = ctx.Manager;
			var createCount = 10;
			var Assert = ctx.Assert;

			var createArguments = new TEditable[createCount];
			for (var i = 0; i < createArguments.Length; i++)
				createArguments[i] = await ctx.Helper.NextSample(new TEditable(), ctx.SampleSeed);

			var createIdents = new TKey[createCount];

			//创建对象
			for (var i = 0; i < createCount; i++)
				createIdents[i] = await svc.CreateAsync(createArguments[i]);


			//检查新创建的对象主键各不相同
			var distincted = createIdents.Distinct(Poco.DeepEqualityComparer<TKey>()).ToArray();
			Assert.Equal("新建的实体主键有重复",distincted.Length, createCount);

			var emptyIdent = new TKey();
			//检查新创建的对象主键不为空
			foreach (var id in createIdents)
				Assert.NotEqual("新建的实体主键为默认值",id, emptyIdent);

			//检查新建结果
			var createResults = new TEditable[createCount];
			for (var i = 0; i < createCount; i++)
			{
				createResults[i] = await svc.LoadForEdit(createIdents[i]);
				Assert.Success("实体创建结果和参数不符",ctx.Helper.ValidateCreateResult(
					createArguments[i],
					createResults[i]
					));
			}



			//检查实体获取
			var details = new TDetail[createCount];
			for (var i = 0; i < createCount; i++)
			{
				details[i] = await svc.GetAsync(createIdents[i]);
				Assert.Success("获取详细实体和可编辑实体不符",ctx.Helper.ValidateDetail(createResults[i], details[i]));
			}

			//检查批量实体获取
			var batchDetails = await svc.BatchGetAsync(createIdents);
			for (var i = 0; i < createCount; i++)
			{
				var key = Entity<TDetail>.GetKey<TKey>(batchDetails[i]);
				var createResult = createResults.SingleOrDefault(r => Poco.DeepEquals(Entity<TEditable>.GetKey<TKey>(r), key));
				 Assert.Success("批量获取的实体和单独获取的实体不符", ctx.Helper.ValidateDetail(createResult, batchDetails[i]));
			}

			var querySeedSummaries = details.Select(d => ctx.Helper.ConvertToSummary(d)).ToArray();
			//检查查询
			foreach (var qa in ctx.Helper.GenerateQueryArgument(querySeedSummaries))
			{
				var re = await svc.QueryAsync(qa.QueryArgument);
				Assert.Equal("查询到的结果和期望不符",re.Items.ToArray(), qa.Results);

				var ire = await svc.QueryIdentsAsync(qa.QueryArgument);
				var eids = qa.Results.Select(i => Entity<TSummary>.GetKey<TKey>(i)).ToArray();
				Assert.Equal("查询到主键和期望不符",ire.Items.ToArray(), eids);
			}


			//检查实体更新
			var updateEditables = new TEditable[createCount];
			var updateResults = new TEditable[createCount];
			for (var i = 0; i < createCount; i++)
			{
				updateEditables[i] =await ctx.Helper.NextSample(createResults[i], ctx.SampleSeed);
				await svc.UpdateAsync(updateEditables[i]);
				updateResults[i] = await svc.LoadForEdit(createIdents[i]);
				Assert.Success("实体更新结果和更新参数不符",ctx.Helper.ValidateUpdateResult(updateEditables[i], updateResults[i]));
			}

			//删除部分
			var removeIds = createIdents.Where((ci, i) => i % 2 == 0).ToArray();
			var restIds = createIdents.Where((ci, i) => i % 2 != 0).ToArray();
			foreach (var rid in removeIds)
			{
				await svc.RemoveAsync(rid);
				Assert.Equal("删除后，还能获取到实体",default(TDetail), await svc.GetAsync(rid));
			}

			//删除剩余部分
			await svc.RemoveAllAsync();
			foreach (var rid in restIds)
				Assert.Equal("批量删除后，还能获取到实体",default(TDetail), await svc.GetAsync(rid));
		}

	}

	public class DefaultEntityTestCaseProvider : ITestCaseProvider
	{
		IEntityMetadataCollection Metadatas { get; }
		IServiceProvider ServiceProvider { get; }
		Dictionary<Type,IAutoTestEntity[]> AutoTestEntities { get; }
		class TestCase : ITestCase
		{
			public string Category { get; set; }
			public string Name { get; set; }
			public IEntityMetadata Metadata { get; set; }
		}
		public DefaultEntityTestCaseProvider(
			IEntityMetadataCollection Metadatas, 
			IServiceProvider ServiceProvider,
			IEnumerable<IAutoTestEntity> AutoTestEntities
			)
		{
			this.AutoTestEntities = AutoTestEntities
				.GroupBy(a => a.EntityManagerType)
				.ToDictionary(g => g.Key, g => g.ToArray());

			this.Metadatas = Metadatas;
			this.ServiceProvider = ServiceProvider;
		}

		
		public IEnumerable<ITestCase> GetTestCases()
		{
			return from e in Metadatas
				   where AutoTestEntities.ContainsKey(e.EntityManagerType)
				   select new TestCase
				   {
					   Category = "实体自动测试",
					   Name = e.Name,
					   Metadata = e
				   };
		}

		async Task Execute<TKey,TDetail,TSummary,TEditable, TQueryArgument, TManager>(ITestCase Case)
			where TManager :
			class,
			IEntitySource<TKey, TSummary, TDetail, TQueryArgument>,
			IEntityManager<TKey, TEditable>
			where TKey : new()
			where TDetail : new()
			where TEditable : new()
			where TQueryArgument : IPagingArgument
		{
			var c = (TestCase)Case;
			var managers = AutoTestEntities[c.Metadata.EntityManagerType];
			foreach (var m in managers)
			{
				await ServiceProvider.WithScope(async isp =>
				{
					await ServiceProvider.TestManagedService<TManager, int>(
						((IAutoTestEntity<TManager>)m).ServiceConfig,
						async svc =>
						{
							var logger=isp.Resolve<ILogger<TManager>>();
							logger.Info($"开始测试{typeof(TManager)}服务基础功能");
							var ctx = ServiceProvider.Resolve<IEntityTestContext<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager>>();
							var executor = new EntityTestExecutor<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager>();
							await executor.Test(ctx);
							return 0;
						}
						);
				});
			}
		}
		static MethodInfo MethodExecute { get; } = typeof(DefaultEntityTestCaseProvider).GetMethodExt(
			"Execute",
			BindingFlags.NonPublic | BindingFlags.Instance,
			typeof(ITestCase)
			).IsNotNull();
		public async Task Execute(ITestCase Case)
		{
			var meta = ((TestCase)Case).Metadata;
			await (Task)MethodExecute.MakeGenericMethod(
					meta.EntityKeyType,
					meta.EntityDetailType,
					meta.EntitySummaryType ?? meta.EntitySummaryType,
					meta.EntityEditableType ?? meta.EntityDetailType,
					meta.QueryArgumentType,
					meta.EntityManagerType
				).Invoke(this, new[] { Case });
			
		}
	}
}

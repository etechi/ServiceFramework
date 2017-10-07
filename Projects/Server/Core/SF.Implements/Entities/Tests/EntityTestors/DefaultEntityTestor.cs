
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.Logging;
using SF.Core.Times;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core.Events;
using SF.ADT;
using SF.Services.Tests;
using System.Reflection;

namespace SF.Entities.Tests.EntityTestors
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
	{

		public override async Task Test(ITestContext testContext)
		{
			var ctx = (IEntityTestContext<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager>)testContext;

			var svc = ctx.Manager;
			var createCount = 10;
			var Assert = ctx.Assert;
			var createArguments = Enumerable.Range(0, createCount)
				.Select(i => ctx.Helper.NextSample(new TEditable(), ctx.SampleSeed)).ToArray();
			var createIdents = new TKey[createCount];

			//创建对象
			for (var i = 0; i < createCount; i++)
				createIdents[i] = await svc.CreateAsync(createArguments[i]);


			//检查新创建的对象主键各不相同
			var distincted = createIdents.Distinct(Poco.DeepEqualityComparer<TKey>()).ToArray();
			Assert.Equal(distincted.Length, createCount);

			var emptyIdent = new TKey();
			//检查新创建的对象主键不为空
			foreach (var id in createIdents)
				Assert.NotEqual(id, emptyIdent);

			//检查新建结果
			var createResults = new TEditable[createCount];
			for (var i = 0; i < createCount; i++)
			{
				createResults[i] = await svc.LoadForEdit(createIdents[i]);
				Assert.Success(ctx.Helper.ValidateCreateResult(
					createArguments[i],
					createResults[i]
					));
			}



			//检查实体获取
			var details = new TDetail[createCount];
			for (var i = 0; i < createCount; i++)
			{
				details[i] = await svc.GetAsync(createIdents[i]);
				Assert.Success(ctx.Helper.ValidateDetail(createResults[i], details[i]));
			}

			//检查批量实体获取
			var batchDetails = await svc.GetAsync(createIdents);
			Assert.Equal(details, batchDetails);


			var querySeedSummaries = details.Select(d => ctx.Helper.ConvertToSummary(d)).ToArray();
			//检查查询
			foreach (var qa in ctx.Helper.GenerateQueryArgument(querySeedSummaries))
			{
				var re = await svc.QueryAsync(qa.QueryArgument, qa.Paging);
				Assert.Equal(re.Items.ToArray(), qa.Results);

				var ire = await svc.QueryIdentsAsync(qa.QueryArgument, qa.Paging);
				var eids = qa.Results.Select(i => Entity<TSummary>.GetKey<TKey>(i)).ToArray();
				Assert.Equal(ire.Items.ToArray(), eids);
			}


			//检查实体更新
			var updateEditables = new TEditable[createCount];
			var updateResults = new TEditable[createCount];
			for (var i = 0; i < createCount; i++)
			{
				updateEditables[i] = ctx.Helper.NextSample(createResults[i], ctx.SampleSeed);
				await svc.UpdateAsync(updateEditables[i]);
				updateResults[i] = await svc.LoadForEdit(createIdents[i]);
				Assert.Success(ctx.Helper.ValidateUpdateResult(updateEditables[i], updateResults[i]));
			}

			//删除部分
			var removeIds = createIdents.Where((ci, i) => i % 2 == 0).ToArray();
			var restIds = createIdents.Where((ci, i) => i % 2 != 0).ToArray();
			foreach (var rid in removeIds)
			{
				await svc.RemoveAsync(rid);
				Assert.Equal(default(TDetail), await svc.GetAsync(rid));
			}

			//删除剩余部分
			await svc.RemoveAllAsync();
			foreach (var rid in restIds)
				Assert.Equal(default(TDetail), await svc.GetAsync(rid));
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
		{
			var c = (TestCase)Case;
			var managers = AutoTestEntities[c.Metadata.EntityManagerType];
			foreach (var m in managers)
			{
				await ServiceProvider.WithScope(async isp =>
				{
					await ServiceProvider.TestService<TManager, int>(
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
			);
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

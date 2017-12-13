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

using SF.Sys.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.Entities.AutoTest
{

	/// <summary>
	/// 实体样本生成器
	/// </summary>
	/// <typeparam name="TEditable"></typeparam>
	public interface IEntitySampleGenerator<TEditable>
	{
		int Priority { get; }
		bool NextSampleSupported { get; }
		Task<TEditable[]> ValidSamples();
		Task<TEditable[]> InvalidSamples();
		Task<TEditable> NextSample(TEditable OrgValue, ISampleSeed Seed);
	}
	public interface IEntitySampleGeneratorProvider
	{
		IEntitySampleGenerator<TEditable> GetSampleGenerator<TEditable>();
	}


	/// <summary>
	/// 验证实体创建结果是否正确
	/// </summary>
	/// <typeparam name="TEditable"></typeparam>
	public interface IEntityCreateResultValidator<TEditable>
	{
		TestResult ValidateCreateResult(TEditable CreateArgument, TEditable UpdateResult);
	}

	public interface IEntityCreateResultValidatorProvider
	{
		IEntityCreateResultValidator<TEditable> GetCreateResultValidator<TEditable>();
	}

	/// <summary>
	/// 验证实体更新结果是否正确
	/// </summary>
	/// <typeparam name="TEditable"></typeparam>
	public interface IEntityUpdateResultValidator<TEditable>
	{
		TestResult ValidateUpdateResult(TEditable UpdateArgument, TEditable UpdateResult);
	}
	public interface IEntityUpdateResultValidatorProvider
	{
		IEntityUpdateResultValidator<TEditable> GetUpdateResultValidator<TEditable>();
	}

	/// <summary>
	/// 验证获取详细实体对象结果是否正确
	/// </summary>
	/// <typeparam name="TEditable"></typeparam>
	/// <typeparam name="TDetail"></typeparam>
	public interface IEntityDetailValidator<TEditable, TDetail>
	{
		TestResult ValidateDetail(TEditable LoadEditableResult, TDetail Detail);
	}
	public interface IEntityDetailValidatorProvider
	{
		IEntityDetailValidator<TEditable, TDetail> GetDetailValidator<TEditable, TDetail>();
	}

	/// <summary>
	/// 验证实体查询结果对象是否正确
	/// </summary>
	/// <typeparam name="TDetail"></typeparam>
	/// <typeparam name="TSummary"></typeparam>
	public interface IEntitySummaryValidator<TDetail, TSummary>
	{
		TestResult ValidateSummary(TDetail Detail, TSummary Summary);
	}
	public interface IEntitySummaryValidatorProvider
	{
		IEntitySummaryValidator<TDetail, TSummary> GetSummaryValidator<TDetail, TSummary>();
	}

	/// <summary>
	/// 将实体详细转换成实体摘要
	/// </summary>
	/// <typeparam name="TDetail"></typeparam>
	/// <typeparam name="TSummary"></typeparam>
	public interface IEntityDetailToSummaryConverter<TDetail, TSummary>
	{
		TSummary ConvertToSummary(TDetail Detail);
	}
	public interface IEntityDetailToSummaryConverterProvider
	{
		IEntityDetailToSummaryConverter<TDetail, TSummary> GetDetailToSummaryConverter<TDetail, TSummary>();
	}

	/// <summary>
	/// 实体查询测试用例
	/// </summary>
	/// <typeparam name="TQueryArgument"></typeparam>
	/// <typeparam name="TSummary"></typeparam>
	public class QueryTestCase<TQueryArgument, TSummary>
	{
		public TQueryArgument QueryArgument { get; set; }
		public IReadOnlyList<TSummary> Results { get; set; }
	}
	/// <summary>
	/// 实体查询参数生成器
	/// </summary>
	/// <typeparam name="TSummary"></typeparam>
	/// <typeparam name="TQueryArgument"></typeparam>
	public interface IEntityQueryArgumentGenerator<TSummary, TQueryArgument> where TQueryArgument:IPagingArgument
	{
		IEnumerable<QueryTestCase<TQueryArgument, TSummary>> GenerateQueryArgument(IEnumerable<TSummary> Summaries);
	}
	public interface IEntityQueryArgumentGeneratorProvider
	{
		IEntityQueryArgumentGenerator<TSummary, TQueryArgument> GetQueryArgumentGenerator<TSummary, TQueryArgument>() where TQueryArgument : IPagingArgument;
	}


}

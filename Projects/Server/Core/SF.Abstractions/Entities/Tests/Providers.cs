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
using SF.Services.Tests;
using System.Threading.Tasks;

namespace SF.Entities.Tests
{
	
	public interface IEntityCreateResultValidator<TEditable>
	{
		TestResult ValidateCreateResult(TEditable CreateArgument, TEditable UpdateResult);
	}

	public interface IEntityCreateResultValidatorProvider
	{
		IEntityCreateResultValidator<TEditable> GetCreateResultValidator<TEditable>();
	}


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


	public interface IEntityUpdateResultValidator<TEditable>
	{
		TestResult ValidateUpdateResult(TEditable UpdateArgument, TEditable UpdateResult);
	}
	public interface IEntityUpdateResultValidatorProvider
	{
		IEntityUpdateResultValidator<TEditable> GetUpdateResultValidator<TEditable>();
	}


	public interface IEntityDetailValidator<TEditable, TDetail>
	{
		TestResult ValidateDetail(TEditable LoadEditableResult, TDetail Detail);
	}
	public interface IEntityDetailValidatorProvider
	{
		IEntityDetailValidator<TEditable, TDetail> GetDetailValidator<TEditable, TDetail>();
	}

	public interface IEntitySummaryValidator<TDetail, TSummary>
	{
		TestResult ValidateSummary(TDetail Detail, TSummary Summary);
	}
	public interface IEntitySummaryValidatorProvider
	{
		IEntitySummaryValidator<TDetail, TSummary> GetSummaryValidator<TDetail, TSummary>();
	}

	public interface IEntityDetailToSummaryConverter<TDetail, TSummary>
	{
		TSummary ConvertToSummary(TDetail Detail);
	}
	public interface IEntityDetailToSummaryConverterProvider
	{
		IEntityDetailToSummaryConverter<TDetail, TSummary> GetDetailToSummaryConverter<TDetail, TSummary>();
	}

	public class QueryTestCase<TQueryArgument, TSummary>
	{
		public TQueryArgument QueryArgument { get; set; }
		public Paging Paging { get; set; }
		public IReadOnlyList<TSummary> Results { get; set; }
	}
	public interface IEntityQueryArgumentGenerator<TSummary, TQueryArgument>
	{
		IEnumerable<QueryTestCase<TQueryArgument, TSummary>> GenerateQueryArgument(IEnumerable<TSummary> Summaries);
	}
	public interface IEntityQueryArgumentGeneratorProvider
	{
		IEntityQueryArgumentGenerator<TSummary, TQueryArgument> GetQueryArgumentGenerator<TSummary, TQueryArgument>();
	}


}

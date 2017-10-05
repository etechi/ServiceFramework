using System;
using System.Collections.Generic;
using System.Linq;
using SF.Services.Tests;

namespace SF.Entities.Tests
{
	
	public interface IEntityCreateResultValidator<TEditable>
	{
		AssertResult ValidateCreateResult(TEditable CreateArgument, TEditable UpdateResult);
	}

	public interface IEntityCreateResultValidatorProvider
	{
		IEntityCreateResultValidator<TEditable> GetCreateResultValidator<TEditable>();
	}


	public interface IEntitySampleGenerator<TEditable>
	{
		int Priority { get; }
		bool NextSampleSupported { get; }
		IEnumerable<TEditable> ValidSamples { get; }
		IEnumerable<TEditable> InvalidSamples { get; }
		TEditable NextSample(TEditable OrgValue, ISampleSeed Seed);
	}
	public interface IEntitySampleGeneratorProvider
	{
		IEntitySampleGenerator<TEditable> GetSampleGenerator<TEditable>();
	}


	public interface IEntityUpdateResultValidator<TEditable>
	{
		AssertResult ValidateUpdateResult(TEditable UpdateArgument, TEditable UpdateResult);
	}
	public interface IEntityUpdateResultValidatorProvider
	{
		IEntityUpdateResultValidator<TEditable> GetUpdateResultValidator<TEditable>();
	}


	public interface IEntityDetailValidator<TEditable, TDetail>
	{
		AssertResult ValidateDetail(TEditable LoadEditableResult, TDetail Detail);
	}
	public interface IEntityDetailValidatorProvider
	{
		IEntityDetailValidator<TEditable, TDetail> GetDetailValidator<TEditable, TDetail>();
	}

	public interface IEntitySummaryValidator<TDetail, TSummary>
	{
		AssertResult ValidateSummary(TDetail Detail, TSummary Summary);
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
	public interface ITestAssert
	{
		void Equal<T>(T expect, T target);
	}
	public interface IEntityTestContext<TKey,TDetail, TSummary, TEditable, TQueryArgument, TManager>
		where TManager:
			IEntitySource<TKey,TSummary,TDetail,TQueryArgument>,
			IEntityManager<TKey,TEditable>
	{
		TManager Manager { get; }
		ITestAssert Assert { get; }
		IEntityTestHelper<TDetail,TSummary,TEditable,TQueryArgument> Helper { get; }
	}

	public interface IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> :
		IEntityCreateResultValidator<TEditable>,
		IEntitySampleGenerator<TEditable>,
		IEntityUpdateResultValidator<TEditable>,
		IEntityDetailValidator<TEditable, TDetail>,
		IEntitySummaryValidator<TDetail, TSummary>,
		IEntityQueryArgumentGenerator<TSummary, TQueryArgument>,
		IEntityDetailToSummaryConverter<TDetail, TSummary>
	{
	}

	public interface IEntityTestHelperCache
	{
		IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> GetTestHelper<TDetail, TSummary, TEditable, TQueryArgument>();
	}


}

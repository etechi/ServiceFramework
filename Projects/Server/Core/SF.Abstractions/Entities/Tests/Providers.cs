using System;
using System.Collections.Generic;
using System.Linq;
using SF.Services.Tests;

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

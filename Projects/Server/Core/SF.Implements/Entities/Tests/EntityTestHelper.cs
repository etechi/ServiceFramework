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
using SF.Services.Tests;

namespace SF.Entities.Tests
{
	class EntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> :
			IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument>
	{
		IEntityDetailToSummaryConverter<TDetail, TSummary> DetailToSummaryConverter { get; }
		IEntitySampleGenerator<TEditable>[] NextSampleGenerators { get; }
		IEntitySampleGenerator<TEditable>[] SamplesGenerators { get; }

		IEntityQueryArgumentGenerator<TSummary, TQueryArgument>[] QueryArgumentGenerators { get; }

		IEntityCreateResultValidator<TEditable>[] CreateResultValidators { get; }
		IEntityDetailValidator<TEditable, TDetail>[] DetailValidators { get; }
		IEntitySummaryValidator<TDetail, TSummary>[] SummaryValidators { get; }
		IEntityUpdateResultValidator<TEditable>[] UpdateResultValidators { get; }

		public int Priority => 0;

		public bool NextSampleSupported => NextSampleGenerators.Length > 0;

		public async Task<TEditable[]> ValidSamples() =>
			(await Task.WhenAll(SamplesGenerators.Select(g => g.ValidSamples()))).SelectMany(m => m).ToArray();

		public async Task<TEditable[]> InvalidSamples() =>
			(await Task.WhenAll(SamplesGenerators.Select(g => g.InvalidSamples()))).SelectMany(m => m).ToArray();

		public EntityTestHelper(
			IEntityDetailToSummaryConverter<TDetail, TSummary> DetailToSummaryConverter,
			IEntitySampleGenerator<TEditable>[] SampleGenerators,
			IEntityQueryArgumentGenerator<TSummary, TQueryArgument>[] QueryArgumentGenerators,
			IEntityCreateResultValidator<TEditable>[] CreateResultValidators,
			IEntityDetailValidator<TEditable, TDetail>[] DetailValidators,
			IEntitySummaryValidator<TDetail, TSummary>[] SummaryValidators,
			IEntityUpdateResultValidator<TEditable>[] UpdateResultValidators
			)
		{
			this.DetailToSummaryConverter = DetailToSummaryConverter;
			this.NextSampleGenerators = SampleGenerators.Where(g => g.NextSampleSupported).ToArray();
			this.SamplesGenerators = SampleGenerators;

			this.QueryArgumentGenerators = QueryArgumentGenerators;

			this.CreateResultValidators = CreateResultValidators;
			this.DetailValidators = DetailValidators;
			this.SummaryValidators = SummaryValidators;
			this.UpdateResultValidators = UpdateResultValidators;

		}
		public TSummary ConvertToSummary(TDetail Detail)
		{
			return DetailToSummaryConverter.ConvertToSummary(Detail);
		}
		public IEnumerable<QueryTestCase<TQueryArgument, TSummary>> GenerateQueryArgument(IEnumerable<TSummary> Summaries)
		{
			return QueryArgumentGenerators.Select(g => g.GenerateQueryArgument(Summaries)).SelectMany(s => s);
		}


		public TestResult ValidateCreateResult(TEditable CreateArgument, TEditable UpdateResult)
		{
			return TestResult.Merge(
				CreateResultValidators.Select(v => v.ValidateCreateResult(CreateArgument, UpdateResult))
				);
		}

		public TestResult ValidateDetail(TEditable LoadEditableResult, TDetail Detail)
		{
			return TestResult.Merge(
				DetailValidators.Select(v => v.ValidateDetail(LoadEditableResult, Detail))
				);
		}

		public TestResult ValidateSummary(TDetail Detail, TSummary Summary)
		{
			return TestResult.Merge(
				SummaryValidators.Select(v => v.ValidateSummary(Detail, Summary))
				);

		}

		public TestResult ValidateUpdateResult(TEditable UpdateArgument, TEditable UpdateResult)
		{
			return TestResult.Merge(
				UpdateResultValidators.Select(v => v.ValidateUpdateResult(UpdateArgument, UpdateResult))
				);
		}

		public async Task<TEditable> NextSample(TEditable OrgValue, ISampleSeed Seed)
		{
			foreach(var g in NextSampleGenerators)
				OrgValue= await g.NextSample(OrgValue, Seed);
			return OrgValue;
			
		}
	}
}

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

		public IEnumerable<TEditable> ValidSamples =>
			SamplesGenerators.SelectMany(g => g.ValidSamples);

		public IEnumerable<TEditable> InvalidSamples =>
			SamplesGenerators.SelectMany(g => g.InvalidSamples);

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
			return QueryArgumentGenerators.Select(g => g(Summaries)).SelectMany(s => s);
		}


		public void ValidateCreateResult(TEditable CreateArgument, TEditable UpdateResult)
		{
			CreateResultValidators.ForEach(v => v.ValidateCreateResult(CreateArgument, UpdateResult));
		}

		public void ValidateDetail(TEditable LoadEditableResult, TDetail Detail)
		{
			DetailValidators.ForEach(v => v.ValidateDetail(LoadEditableResult, Detail));
		}

		public void ValidateSummary(TDetail Detail, TSummary Summary)
		{
			SummaryValidators.ForEach(v => v.ValidateSummary(Detail, Summary));
		}

		public void ValidateUpdateResult(TEditable UpdateArgument, TEditable UpdateResult)
		{
			UpdateResultValidators.ForEach(v => v.ValidateUpdateResult(UpdateArgument, UpdateResult));
		}

		int index = 0;
		public TEditable NextSample(TEditable OrgValue, ISampleSeed Seed)
		{
			index++;
			return NextSampleGenerators[index % NextSampleGenerators.Length].NextSample(OrgValue, Seed);
			
		}
	}
}

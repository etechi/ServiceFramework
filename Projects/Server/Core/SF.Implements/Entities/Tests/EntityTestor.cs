
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

namespace SF.Entities.Tests
{
	class EntityTestor<TDetail, TSummary, TEditable, TQueryArgument, TManager> :
			IEntityTestor<TDetail, TSummary, TEditable, TQueryArgument,TManager>
	{

		public EntityTestContext(
			IEntityDetailToSummaryConverter<TDetail, TSummary> DetailToSummaryConverter,
			IEntityToucher<TEditable>[] EntityTouchers,
			IEntityQueryArgumentGenerator<TSummary, TQueryArgument>[] QueryArgumentGenerators,

			IEntityCreateResultValidator<TEditable>[] CreateResultValidators,
			IEntityDetailValidator<TEditable, TDetail>[] DetailValidators,
			IEntitySummaryValidator<TDetail, TSummary>[] SummaryValidators,
			IEntityUpdateResultValidator<TEditable>[] UpdateResultValidators
			)
		{
			this.DetailToSummaryConverter = DetailToSummaryConverter;
			this.EntityTouchers = EntityTouchers;
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

		public TEditable Touch(TEditable editable, ITestValueGenerator ValueGenerator)
		{
			return EntitySampleGenerators.Aggregate(editable, (x, i) => i.Touch(x, ValueGenerator));
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
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Data
{
	public interface ISummary
    {
    }
	public interface IQueryResult
	{
		 ISummary Summary { get; set; }
		 int? Total { get; set; }
		 IEnumerable Items { get; set; }
	}
	public class QueryResult<T> : IQueryResult,ISummary
	{
        public ISummary Summary { get; set; }
        public int? Total { get; set; }
        public IEnumerable<T> Items { get; set; }
        public static QueryResult<T> Empty { get; } = new QueryResult<T>
        {
            Items = Enumerable.Empty<T>()
        };
		IEnumerable IQueryResult.Items { get => Items; set { Items = (IEnumerable<T>)value; } }
	}
}

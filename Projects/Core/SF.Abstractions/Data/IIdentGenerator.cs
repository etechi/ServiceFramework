using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	[Metadata.Comment("对象标识生成器")]
	public interface IIdentGenerator
	{
        Task<long> GenerateAsync(string Type);
	}
}

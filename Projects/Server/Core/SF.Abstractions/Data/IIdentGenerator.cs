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
        Task<long> GenerateAsync(string Type,int Section=0);
	}

	public interface IIdentGenerator<T>: IIdentGenerator
	{
		Task<long> GenerateAsync(int Section=0);
	}
}

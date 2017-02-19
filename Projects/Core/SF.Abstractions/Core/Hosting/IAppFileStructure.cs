using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Hosting
{
    public interface IAppFileStructure
    {
		string RootPath { get; }
		string ContentPath { get; }
		string TempPath { get; }
		string DataPath { get; }
		string BinaryPath { get; }
	}
}

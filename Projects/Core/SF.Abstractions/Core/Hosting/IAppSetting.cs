using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Hosting
{
    public interface IAppSetting<TSetting> where TSetting:class,new()
    {
		TSetting Value { get; }
	}
}

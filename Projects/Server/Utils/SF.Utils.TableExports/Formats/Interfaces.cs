using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Utils.TableExports
{
    public interface IFormatResolver
    {
        IFormat Resolve(Type type);
    }
    public interface IFormat
    {
        object Format(object value);
    }
}

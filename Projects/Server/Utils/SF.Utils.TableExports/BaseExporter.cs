using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Utils.TableExports
{
    
    public class BaseTableExporter 
    {
        protected Column[] Columns { get; }
        protected IFormat[] Formats { get; }
        public BaseTableExporter(Column[] Columns)
        {
            this.Columns = Columns;
            this.Formats = Columns.Select(c => DefaultFormatResolver.Instance.Resolve(c.Type)).ToArray();

        }
        protected object Format(int Index,object Value)
        {
            return Formats[Index].Format(Value);
        }
    }
}

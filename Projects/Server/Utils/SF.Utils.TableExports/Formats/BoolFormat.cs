using System;
namespace SF.Utils.TableExports
{

	public class BoolFormat : IFormat
    {
        public static BoolFormat Instance { get; } = new BoolFormat();
        public object Format(object value)
        {
            return value == null || !Convert.ToBoolean(value) ? "否" : "是";
        }
    }
}

using System;
namespace SF.Utils.TableExports
{

	public class CommonFormat : IFormat
    {
        public static IFormat Instance { get; } = new CommonFormat();
        public object Format(object value)
        {
            return value == null ? string.Empty : Convert.ToString(value);
        }
    }
}

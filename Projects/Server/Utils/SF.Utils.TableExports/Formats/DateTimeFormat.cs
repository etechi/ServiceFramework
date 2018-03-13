namespace SF.Utils.TableExports
{

	public class DateTimeFormat : IFormat
    {
        public static IFormat Instance { get; } = new DateTimeFormat();
        public object Format(object value)
        {
            if (value == null)
                return string.Empty;
            return value;
            //var t = Convert.ToDateTime(value);
            //return t.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}

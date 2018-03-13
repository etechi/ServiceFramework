namespace SF.Utils.TableExports
{

	public class NumberFormat : IFormat
    {
        public static NumberFormat Instance = new NumberFormat();

        public object Format(object value)
        {
            return value;
        }
    }
}

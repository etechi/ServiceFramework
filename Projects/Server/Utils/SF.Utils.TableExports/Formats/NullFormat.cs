namespace SF.Utils.TableExports
{

	public class NullFormat : IFormat
    {
        public static NullFormat Instance { get; } = new NullFormat();
        public object Format(object value)
        {
            return null;
        }
    }
}

namespace SF.Utils.TableExports
{

	public class NullableFormat<T> : IFormat
        where T:struct
    {
        IFormat BaseFormat { get; }
        
        public NullableFormat(IFormat BaseFormat)
        {
            this.BaseFormat = BaseFormat;
        }
        public object Format(object value)
        {
            var n = (T?)value;
            if (!n.HasValue)
                return string.Empty;
            return this.BaseFormat.Format(n.Value);
        }
    }
}

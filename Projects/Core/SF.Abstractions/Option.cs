namespace SF
{
	public struct Option<T>
	{
		public T Value { get; set; }
		public bool HasValue { get; set; }
	}
	public static class OptionExtension
	{
		public static T ValueOrDefault<T>(this Option<T> value)
		{
			return value.HasValue ? value.Value : default(T);
		}
	}

}

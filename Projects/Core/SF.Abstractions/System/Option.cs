namespace System
{
	public struct Option<T>
	{
		public T Value { get; set; }
		public bool HasValue { get; set; }

		public static implicit operator Option<T>(T value)
		{
			return new Option<T>
			{
				Value = value,
				HasValue = true
			};
		}
	}
	public static class OptionExtension
	{
		public static T ValueOrDefault<T>(this Option<T> value)
		{
			return value.HasValue ? value.Value : default(T);
		}
	}

}

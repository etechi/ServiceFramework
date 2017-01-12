namespace SF
{
	public struct Option<T>
	{
		public T Value { get; set; }
		public bool HasValue { get; set; }
	}
}

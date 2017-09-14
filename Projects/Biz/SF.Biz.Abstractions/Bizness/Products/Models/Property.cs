using SF.Entities;

namespace SF.Biz.Products
{

	public class Property
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }
		public string Image { get; set; }
		public Property[] Children { get; set; }
	}
	public class PropertyScope
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }
		public string Image { get; set; }
		public Property[] Properties { get; set; }
	}
	public class PropertyInternal : Property
	{
		public string Name { get; set; }
		public EntityLogicState ObjectState { get; set; }
	}
	public class PropertyScopeInternal : PropertyScope
	{
		public string Name { get; }
		public EntityLogicState ObjectState { get; set; }
	}
}

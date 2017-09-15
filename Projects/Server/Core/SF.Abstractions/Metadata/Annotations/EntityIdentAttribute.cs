using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SF.Entities;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
	public class EntityIdentAttribute : Attribute
	{
		public Type EntityManagementType { get; }
		public string NameField { get; }
		public int Column { get; }
		public string ScopeField { get; set; }
		public object ScopeValue { get; set; }
		public bool IsTreeParentId { get; set; }
		public string MultipleKeyField { get; set; }

		public EntityIdentAttribute(Type EntityManagementType = null, string NameField = null, int Column = 0, string MultipleKeyField = null)
		{
			//if(EntityManagementType != null)
			//{
			//	if (EntityManagementType.IsGenericTypeDefinition)
			//		throw new NotSupportedException();

			//	var loadable = GetAllInterfaces(EntityManagementType)
			//		.Where(t =>
			//			t.IsInterface &&
			//			t.IsGenericType &&
			//			t.GetGenericTypeDefinition() == typeof(IEntityLoadable<,>)
			//			)
			//		.FirstOrDefault();
			//	if (loadable == null)
			//		throw new NotSupportedException($"指定的类型{EntityManagementType}未实现IEntityLoadable<,>接口");


			//	this.EntityManagementType = EntityManagementType;
			//}

			this.EntityManagementType = EntityManagementType;
			this.NameField = NameField;
			this.Column = Column;
			this.MultipleKeyField = MultipleKeyField;
		}
	}
	
}

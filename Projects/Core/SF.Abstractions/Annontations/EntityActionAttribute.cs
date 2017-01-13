using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	public enum ActionMode
	{
		/// <summary>
		/// 编辑模式
		/// </summary>
		Edit,
		/// <summary>
		/// 载入模式
		/// </summary>
		Post,
		/// <summary>
		/// 更新模式
		/// </summary>
		Update,
		/// <summary>
		/// 查询模式
		/// </summary>
		Query
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class EntityActionAttribute : Attribute
	{
		public ActionMode Mode { get; set; }
		public string ConditionExpression { get; set; }
	}

}

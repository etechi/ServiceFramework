using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ImageAttribute : MediaAttribute
	{
		public bool Small { get; set; }
		public ImageAttribute(string[] mimes = null) :
			base(mimes ?? new[] { "image/jpeg", "image/png" })
		{

		}
	}


}

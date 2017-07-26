﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public interface IUIEntity : IEntity
	{
		string Title { get; set; }

		string SubTitle { get; set; }

		string Remarks { get; set; }

		string Description { get; set; }

		string Image { get; set; }

		string Icon { get; set; }
	}
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class UserQueryArgument : Data.Services.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string NickName { get; set; }
	}
	public interface IUserAdminService : 
		Data.Services.IEntitySource<long,UserInternal,UserQueryArgument>,
		Data.Services.IEntityManager<long,UserEditable>
    {
    }

}


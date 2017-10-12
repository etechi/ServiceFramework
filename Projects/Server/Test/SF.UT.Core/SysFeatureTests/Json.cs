using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using Xunit.Sdk;
using SF.Users.Promotions.MemberInvitations;

namespace SF.UT.SysFeatureTests
{
	public class JsonTest
	{
		[Fact]
		public void DynamicTest()
		{
			var re = Newtonsoft.Json.JsonConvert.DeserializeObject("{a:{b:1,c:2},d:3}");
			Assert.NotNull(re);
		}
	}
	

}

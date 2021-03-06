#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
using SF.Promotions.MemberInvitations;

namespace SF.UT.SysFeatureTests
{
	public class XUnitTest
	{
		public class CheckThisNumber
		{
			public int IntialValue { get; set; }

			public CheckThisNumber(int intialValue)
			{
				IntialValue = intialValue;
			}

			public bool CheckIfEqual(int input)
			{
				return IntialValue == input;
			}
		}
		public static class DemoPropertyDataSource
		{
			private static readonly List<object[]> _data
				= new List<object[]>
					{
					new object[] {1, true},
					new object[] {2, false},
					new object[] {-1, false},
					new object[] {0, false},
					new object[] {4, false}
					};

			public static IEnumerable<object[]> TestData
			{
				get {

					return _data;
				}
			}
		}
		
		[Theory]
		[MemberData("TestData", MemberType = typeof(DemoPropertyDataSource))]
		public void SampleTest1(int number, bool expectedResult)
		{
			var sut = new CheckThisNumber(1);
			var result = sut.CheckIfEqual(number);
			Assert.Equal(result, expectedResult);
		}

		public interface IEntityServiceTestCase
		{
			string Name { get; }
			Task Execute(IServiceProvider ServiceProvider, object Service);
		}

		class EntityServiceTestCase : IEntityServiceTestCase
		{
			public string Name { get; set; }
			public override int GetHashCode()
			{
				return Name.GetHashCode();
			}
			public override bool Equals(object obj)
			{
				var r = obj as EntityServiceTestCase;
				if (r == null) return false;
				return r.Name == Name;
			}
			public override string ToString()
			{
				return Name;
			}
			public Task Execute(IServiceProvider ServiceProvider,object Service)
			{
				return Task.CompletedTask;
			}
		}

		class EntityServiceTestor
		{
			public async Task Test(IEntityServiceTestCase TestCase, IServiceProvider ServiceProvider, object Service)
			{
				await TestCase.Execute(ServiceProvider, Service);
			}
		}

		public  class EntityServiceTestCaseSourceAttribute : DataAttribute
		{
			public EntityServiceTestCaseSourceAttribute(Type ServiceInterface)
			{
			}

			public override IEnumerable<object[]> GetData(MethodInfo testMethod)
			{
				//throw new ArgumentException("xxaaa");
				return new[]{
					new object[]{new EntityServiceTestCase { Name = "case1" } },
					new object[]{new EntityServiceTestCase { Name = "case2" }},
					new object[]{new EntityServiceTestCase { Name = "case3" }},
					new object[]{new EntityServiceTestCase { Name = "case4" }},
					new object[]{new EntityServiceTestCase { Name = "case5" } },
					};
			}
		}

		[Theory]
		[EntityServiceTestCaseSource(typeof(IMemberInvitationManagementService))]
		public async Task TestEntityService1(IEntityServiceTestCase c)
		{
			var testor = new EntityServiceTestor();
			await testor.Test(c,null,null);
		}
	}
	

}

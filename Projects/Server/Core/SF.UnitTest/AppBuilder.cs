using System;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace SF.UTest.AB
{
	

	//public class Item
	//{
	//	public string Name { get; set; }
	//}
	//public class ApplicationTemplate
	//{
	//	public string Name { get; set; }
	//	public Item[] Items { get; set; }
	//}

	//public class Category : Item
	//{
	//	public Item[] Children { get; set; }
	//}
	//public class Feature : Item
	//{
	//	public Item[] Entities { get; set; }
	//}
	//public class Entity<I>  : Feature
	//{
	//	public int Scope { get; set; }
	//	public string Ident { get; set; }
	//}
	//public class Application
	//{

	//}
	//public class AppBuilder
	//{
	//	public static Application BuildApplication(params Item[] Items)
	//	{
	//		return null;
	//	}
	//	public static Category Category(string Name, params Item[] Items)
	//	{
	//		return null;
	//	}
	//	public static Feature Feature<I>(string Name,params Item[] Items)
	//	{
	//		return null;
	//	}
	//}
 //   public class AppBuilderTest
 //   {
	//	public void Test()
	//	{
	//		var app = BuildApplication(
	//			new Category
	//			{
	//				Name = "cat1",
	//				Children = new Item[]
	//				{
	//					new Feature<string>
	//					{
	//						Name="feature1"
	//					}
	//				}
	//			},
	//			new Feature<string>
	//			{
	//				Name = "feature1"
	//			},
	//			new Feature<string>
	//			{
	//				Name = "feature1"
	//			}
	//		);

	//	}		

	//}
}

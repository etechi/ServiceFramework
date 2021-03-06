﻿#region Apache License Version 2.0
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

using SF.Sys.Comments;
using SF.Sys.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Metadata
{
	public interface IMetadataTypeCollection
	{
		Models.Type FindType(string Name);
		void AddType(Models.Type type);
		IEnumerable<Models.Type> GetTypes();
	}
	public interface IMetadataAttributeValuesProvider
	{
		object GetValues(System.Attribute Attribute, object AttrSource);
	}
	public interface IMetadataAttributeValuesProvider<T> : IMetadataAttributeValuesProvider
	{ }
	public interface IMetadataBuilder
	{
		IMetadataTypeCollection TypeCollection { get; }
		IJsonSerializer JsonSerializer { get; }
		string FormatTypeName(Type type);
		Models.Type TryGenerateAndAddType(Type type);
		Models.Type GenerateAndAddType(Type type);
		Models.Property GenerateTypeProperty(System.Reflection.PropertyInfo prop, object DefaultValueObject);
		Models.Property[] GenerateTypeProperties(Type type);
		T LoadAttributes<T>(T item, IEnumerable<Attribute> attrs, object attrSource,Comment Comment, Predicate<Attribute> predicate = null) where T : Models.Entity;
	}
}

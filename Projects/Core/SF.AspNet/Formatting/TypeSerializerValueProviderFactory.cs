using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using System.Web;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using System.Globalization;
using System.Dynamic;
using System.Collections;
using SF.Core.Serialization;

namespace SF.AspNet.Formatting
{

	public class TypeSerializerValueProviderFactory : ValueProviderFactory
	{
		public IJsonSerializer Serializer { get; }
		public string Mime { get; }
		public TypeSerializerValueProviderFactory(IJsonSerializer Serializer, string Mime = null)
		{
			this.Mime = Mime ?? "application/json";
			this.Serializer = Serializer;
		}
		public override IValueProvider GetValueProvider(ControllerContext controllerContext)
		{
			// first make sure we have a valid context
			if (controllerContext == null)
				throw new ArgumentNullException("controllerContext");
			var req = controllerContext.HttpContext.Request;

			// now make sure we are dealing with a json request
			if (!req.ContentType.StartsWith(Mime, StringComparison.OrdinalIgnoreCase))
				return null;
			if (req.ContentLength == 0)
				return null;
			var content = req.ContentEncoding.GetString(req.BinaryRead(req.ContentLength));
			if (content.Length == 0)
				return null;

			Object JSONObject;
			// if we start with a "[", treat this as an array
			if (content[0] == '[')
				JSONObject = (List<ExpandoObject>)Serializer.Deserialize(content, typeof(List<ExpandoObject>));
			else
				JSONObject = Serializer.Deserialize(content,typeof(ExpandoObject));

			// create a backing store to hold all properties for this deserialization
			var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			// add all properties to this backing store
			AddToBackingStore(backingStore, String.Empty, JSONObject);
			// return the object in a dictionary value provider so the MVC understands it
			return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
		}

		private static void AddToBackingStore(Dictionary<string, object> backingStore, string prefix, object value)
		{
			var d = value as IDictionary<string, object>;
			if (d != null)
			{
				foreach (var entry in d)
				{
					AddToBackingStore(backingStore, MakePropertyKey(prefix, entry.Key), entry.Value);
				}
				return;
			}

			var l = value as IList;
			if (l != null)
			{
				for (var i = 0; i < l.Count; i++)
				{
					AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
				}
				return;
			}

			// primitive
			backingStore[prefix] = value;
		}

		private static string MakeArrayKey(string prefix, int index)
		{
			return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
		}

		private static string MakePropertyKey(string prefix, string propertyName)
		{
			return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
		}
	}

}

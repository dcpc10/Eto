using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Eto.Json
{
	public class PropertyStoreConverter : JsonConverter
	{
		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException ();
		}

		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var store = existingValue as PropertyStore;
			var items = JArray.ReadFrom (reader);

			foreach (var item in (IEnumerable<JToken>)items)
			{
				var typeName = (string)item["$type"];
				if (typeName != null) {

					var type = ((EtoBinder)serializer.Binder).BindToType (typeName);
					if (type != null) {
						foreach (var prop in (IDictionary<string, JToken>)item) {
							if (prop.Key == "$type") continue;
							var memberName = "Set" + prop.Key;
							var member = type.GetMethod(memberName, BindingFlags.Static | BindingFlags.Public);
							if (member == null)
								throw new JsonSerializationException(string.Format ("Could not find attachable property {0}.{1}", type.Name, memberName));
							var parameters = member.GetParameters();
							if (parameters.Length != 2)
								throw new JsonSerializationException("Invalid number of parameters");
							var propType = parameters[1].ParameterType;
							using (var propReader = new JTokenReader(prop.Value)) {
								var propValue = serializer.Deserialize(propReader, propType);
								member.Invoke (null, new object[] { store.Parent, propValue });
							}
						}
					}
				}
			}
			return existingValue;
		}

		public override bool CanConvert (Type objectType)
		{
			return typeof(PropertyStore).IsAssignableFrom(objectType);
		}
	}
}

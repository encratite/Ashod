using Newtonsoft.Json.Serialization;
using System.Linq;

namespace Ashod.WebSocket
{
	class JavaScriptContractResolver : DefaultContractResolver
	{
		protected override string ResolvePropertyName(string propertyName)
		{
			return GetJavaScriptName(propertyName);
		}

		public static string GetJavaScriptName(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				return propertyName;
			string newPropertyName = char.ToLower(propertyName[0]) + propertyName.Substring(1);
			return newPropertyName;
		}
	}
}

using System.IO;
using System.Xml.Serialization;

namespace Ashod
{
	public static class XmlFile
	{
		public const string DefaultPath = "Configuration.xml";

		public static void Write<SerializedType>(SerializedType input, string path = DefaultPath)
		{
			using (var stream = new StreamWriter(path))
			{
				var serialiser = new XmlSerializer(typeof(SerializedType));
				serialiser.Serialize(stream, input);
			}
		}

		public static SerializedType Read<SerializedType>(string path = DefaultPath)
		{
			using (var stream = new StreamReader(path))
			{
				var serialiser = new XmlSerializer(typeof(SerializedType));
				var output = (SerializedType)serialiser.Deserialize(stream);
				return output;
			}
		}
	}
}

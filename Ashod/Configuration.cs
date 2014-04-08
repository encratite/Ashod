using System.IO;
using System.Xml.Serialization;

namespace Ashod
{
	public static class Configuration
	{
		public const string DefaultPath = "Configuration.xml";

		public static void Write<ConfigurationType>(ConfigurationType input, string path = DefaultPath)
		{
			using (var stream = new StreamWriter(path))
			{
				var serialiser = new XmlSerializer(typeof(ConfigurationType));
				serialiser.Serialize(stream, input);
			}
		}

		public static ConfigurationType Read<ConfigurationType>(string path = DefaultPath)
		{
			using (var stream = new StreamReader(path))
			{
				var serialiser = new XmlSerializer(typeof(ConfigurationType));
				var output = (ConfigurationType)serialiser.Deserialize(stream);
				return output;
			}
		}
	}
}

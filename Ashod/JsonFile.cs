using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Ashod
{
    public static class JsonFile
    {
        public const string DefaultPath = "Configuration.json";

        public static void Write<SerializedType>(SerializedType input, string path = DefaultPath)
        {
            using (var stream = new StreamWriter(path))
            {
                var serializer = new DataContractJsonSerializer(typeof(SerializedType));
                serializer.WriteObject(stream.BaseStream, input);
            }
        }

        public static SerializedType Read<SerializedType>(string path = DefaultPath)
        {
			string content = File.ReadAllText(path);
			var bytes = Encoding.UTF8.GetBytes(content);
			using (var stream = new MemoryStream(bytes))
			{
				var serializer = new DataContractJsonSerializer(typeof(SerializedType));
				var output = (SerializedType)serializer.ReadObject(stream);
				return output;
			}
        }
    }
}

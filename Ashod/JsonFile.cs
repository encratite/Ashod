using System.IO;
using System.Runtime.Serialization.Json;

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
            using (var stream = new StreamReader(path))
            {
                var serializer = new DataContractJsonSerializer(typeof(SerializedType));
                var output = (SerializedType)serializer.ReadObject(stream.BaseStream);
                return output;
            }
        }
    }
}

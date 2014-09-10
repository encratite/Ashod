using Newtonsoft.Json;
using System.IO;

namespace Ashod
{
    public static class JsonFile
    {
        public const string DefaultPath = "Configuration.json";

        public static void Write<SerializedType>(SerializedType input, string path = DefaultPath)
        {
            string data = JsonConvert.SerializeObject(input);
            File.WriteAllText(path, data);
        }

        public static SerializedType Read<SerializedType>(string path = DefaultPath)
        {
            string data = File.ReadAllText(path);
            var output = JsonConvert.DeserializeObject<SerializedType>(data);
            return output;
        }
    }
}

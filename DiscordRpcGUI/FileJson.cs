using System.IO;
using Newtonsoft.Json;

namespace DiscordRpcGUI
{
    public static class FileJson
    {
        public static T Read<T>(string path)
        {
            if (!File.Exists(path))
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            catch
            {

            }

            return default;
        }

        public static void Write(string path, object o)
        {
            if (!File.Exists(path))
                return;

            string json = JsonConvert.SerializeObject(o, Formatting.Indented);

            File.WriteAllText(path, json);
        }
    }
}

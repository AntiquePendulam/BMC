using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace BMC2
{
    [JsonObject]
    public class Data
    {
        [JsonProperty]
        public static string AccessToken { get; set; }

        [JsonProperty]
        public static string SecretToken { get; set; }

        [JsonIgnore]
        public static string ConsumerKey { get; private set; } = "{ConsumerKey}";

        [JsonIgnore]
        public static string ConsumerSecret { get; private set; } = "{ConsumerSecretKey}";

        public static void Save()
        {
            var s = JsonConvert.SerializeObject(new Data(), Formatting.Indented);
            using (var stream = new StreamWriter(MainWindow.SettingFilePath, false, Encoding.UTF8))
            {
                stream.Write(s);
            }
        }

    }
}

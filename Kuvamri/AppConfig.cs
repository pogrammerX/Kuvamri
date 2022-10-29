using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuvamri
{
    public static class AppConfig
    {
        struct appCfgDataRec
        {
            [JsonProperty("err404")]
            public string e404;
            [JsonProperty("err403")]
            public string e403;
            [JsonProperty("err500")]
            public string e500;
            [JsonProperty("favicon")]
            public string fav;
        }

        public static string Error404Page { get; private set; }

        public static string Error403Page { get; private set; }

        public static string Error500Page { get; private set; }

        public static string Favicon { get; private set; }

        public static void Load(string dir)
        {
            appCfgDataRec rec = JsonConvert.DeserializeObject<appCfgDataRec>(File.ReadAllText(dir + "\\app.config"));

            Error404Page = dir + "\\" + rec.e404;
            Error403Page = dir + "\\" + rec.e403;
            Error500Page = dir + "\\" + rec.e500;
            Favicon = dir + "\\" + rec.fav;
        }
    }
}

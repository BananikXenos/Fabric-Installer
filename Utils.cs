using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AtlasClient.FabricInstallerUtil
{
    public class Utils
    {
        public static void DownloadFile(Uri uri, string outputPath)
        {
            using (WebClient webClient = new WebClient())
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                webClient.DownloadFile(uri, outputPath);
            }
        }

        public static string ReadString(Uri uri)
        {
            using(WebClient webClient = new WebClient())
            {
                return webClient.DownloadString(uri);
            }
        }

        public static byte[] ConvertJsonDocumentToJsonBytes(JsonDocument jsonDocument)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
                {
                    jsonDocument.WriteTo(writer);
                }

                return stream.ToArray();
            }
        }
    }
}

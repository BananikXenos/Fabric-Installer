using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AtlasClient.FabricInstallerUtil
{
    public sealed class FabricService
    {
        private static int activeIndex = 0;
        private static FabricService fixedService;

        private readonly string meta;
        private readonly string maven;

        public FabricService(string meta, string maven)
        {
            this.meta = meta;
            this.maven = maven;
        }

        public string GetMetaUrl()
        {
            return meta;
        }

        public string GetMavenUrl()
        {
            return maven;
        }

        public override string ToString()
        {
            return $"FabricService{{meta='{meta}', maven='{maven}'}}";
        }

        public static JsonDocument QueryMetaJson(string path)
        {
            return InvokeWithFallbacks<JsonDocument, string>((service, arg) =>
            {
                string jsonContent = Utils.ReadString(new Uri(service.meta + arg));
                return JsonDocument.Parse(jsonContent);
            }, path);
        }

        public static JsonDocument QueryJsonSubstitutedMaven(string url)
        {
            if (!url.StartsWith("https://maven.fabricmc.net/"))
                return JsonDocument.Parse(Utils.ReadString(new Uri(url)));

            string path = url.Substring("https://maven.fabricmc.net/".Length);
            return InvokeWithFallbacks<JsonDocument, string>((service, arg) =>
            {
                string jsonContent = Utils.ReadString(new Uri(service.maven + arg));
                return JsonDocument.Parse(jsonContent);
            }, path);
        }

        public static void DownloadSubstitutedMaven(string url, string outputPath)
        {
            if (!url.StartsWith("https://maven.fabricmc.net/"))
            {
                Utils.DownloadFile(new Uri(url), outputPath);
                return;
            }

            string path = url.Substring("https://maven.fabricmc.net/".Length);
            InvokeWithFallbacks<object, string>((service, arg) =>
            {
                Utils.DownloadFile(new Uri(service.maven + arg), outputPath);
                return null;
            }, path);
        }

        private static R InvokeWithFallbacks<R, A>(Handler<A, R> handler, A arg) where R : class
        {
            if (fixedService != null)
                return handler.Invoke(fixedService, arg);

            int index = activeIndex;
            Exception exc = null;

            while (true)
            {
                FabricService service = Reference.FabricServices[index];
                try
                {
                    R ret = handler.Invoke(service, arg);
                    activeIndex = index;
                    return ret;
                }
                catch (IOException e)
                {
                    Console.WriteLine($"service {service} failed: {e}");
                    if (exc == null)
                        exc = e;
                    else
                        exc.Data.Add("SuppressedException", e);
                    index = (index + 1) % Reference.FabricServices.Length;
                    if (index == activeIndex)
                        break;
                }
            }

            throw exc;
        }

        public static void SetFixed(string metaUrl, string mavenUrl)
        {
            if (metaUrl == null && mavenUrl == null)
                throw new ArgumentNullException("Both meta and maven URLs are null");

            if (metaUrl == null)
                metaUrl = "https://meta.fabricmc.net/";

            if (mavenUrl == null)
                mavenUrl = "httpsmaven.fabricmc.net/";

            activeIndex = -1;
            fixedService = new FabricService(metaUrl, mavenUrl);
        }

        private delegate R Handler<A, R>(FabricService service, A arg) where R : class;
    }
}

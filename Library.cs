using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AtlasClient.FabricInstallerUtil
{
    public class Library : IDisposable
    {
        public string name { get; private set; }
        public string url { get; private set; }
        public string inputPath { get; private set; }

        public Library(string name, string url, string inputPath)
        {
            this.name = name;
            this.url = url;
            this.inputPath = inputPath;
        }

        public Library(JsonElement json)
        {
            this.name = json.GetProperty("name").GetString();
            this.url = json.GetProperty("url").GetString();
            this.inputPath = null;
        }

        public string GetURL()
        {
            string[] parts = this.name.Split(new char[] { ':' }, 3);
            string path = $"{parts[0].Replace(".", "/")}/{parts[1]}/{parts[2]}/{parts[1]}-{parts[2]}.jar";
            return this.url + path;
        }

        public string GetPath()
        {
            string[] parts = this.name.Split(new char[] { ':' }, 3);
            string path = $"{parts[0].Replace(".", Path.DirectorySeparatorChar.ToString())}{Path.DirectorySeparatorChar}{parts[1]}{Path.DirectorySeparatorChar}{parts[2]}{Path.DirectorySeparatorChar}{parts[1]}-{parts[2]}.jar";
            return Regex.Replace(path, " ", "_");
        }

        public void Dispose()
        {
            this.name = null;
            this.url = null;
            this.inputPath = null;
        }
    }
}

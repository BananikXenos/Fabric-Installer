using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AtlasClient.FabricInstallerUtil
{
    public class FabricInstaller
    {
        public static readonly MetaHandler GAME_VERSION_META = new MetaHandler("v2/versions/game");

        public static readonly MetaHandler LOADER_META = new MetaHandler("v2/versions/loader");

        public delegate void ProgressUpdateHandler(int percentage);
        public event ProgressUpdateHandler OnPercentageChange;

        public delegate void CompletionHandler();
        public event CompletionHandler OnCompleted;

        public readonly string MinecraftDirectory;
        public readonly string MinecraftVersion;
        private readonly string FabricVersion;

        public FabricInstaller(string minecraftDirectory, string minecraftVersion, string fabricVersion)
        {
            MinecraftDirectory = minecraftDirectory;
            MinecraftVersion = minecraftVersion;
            FabricVersion = fabricVersion;
        }

        public void Install()
        {
            Console.WriteLine("Installing " + MinecraftVersion + " with fabric " + FabricVersion);
            string profileName = string.Format("{0}-{1}-{2}", "fabric-loader", FabricVersion, MinecraftVersion);
            string versionsDir = Path.Combine(MinecraftDirectory, "versions");
            string profileDir = Path.Combine(versionsDir, profileName);
            string profileJson = Path.Combine(profileDir, profileName + ".json");

            if (!File.Exists(profileJson))
            {
                Directory.CreateDirectory(profileDir);
            }

            string dummyJar = Path.Combine(profileDir, profileName + ".jar");

            if (File.Exists(dummyJar))
            {
                File.Delete(dummyJar);
            }

            using (File.Create(dummyJar))
            {
                JsonDocument json = FabricService.QueryMetaJson(string.Format("v2/versions/loader/{0}/{1}/profile/json", MinecraftVersion, FabricVersion));

                using (FileStream fileStream = new FileStream(profileJson, FileMode.Create))
                {
                    byte[] jsonBytes = Utils.ConvertJsonDocumentToJsonBytes(json);
                    fileStream.Write(jsonBytes, 0, jsonBytes.Length);
                }

                string libsDir = Path.Combine(MinecraftDirectory, "libraries");
                int libraryCount = json.RootElement.GetProperty("libraries").GetArrayLength();
                int libraryIndex = 0;

                foreach (JsonElement libraryJson in json.RootElement.GetProperty("libraries").EnumerateArray())
                {
                    using (Library library = new Library(libraryJson))
                    {
                        string libraryFile = Path.Combine(libsDir, library.GetPath());
                        string url = library.GetURL();
                        FabricService.DownloadSubstitutedMaven(url, libraryFile);

                        // Notify percentage change
                        libraryIndex++;
                        int percentage = (int)((float)libraryIndex / libraryCount * 100);
                        OnPercentageChange?.Invoke(percentage);
                    }
                }
            }

            Console.WriteLine("Done installing " + MinecraftVersion + " with fabric " + FabricVersion);

            // Notify completion
            OnCompleted?.Invoke();
        }
    }
}

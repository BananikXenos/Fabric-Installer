using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AtlasClient.FabricInstallerUtil
{
    public class GameVersion
    {
        private string version;
        private bool stable;

        public GameVersion(JsonElement json)
        {
            this.version = json.GetProperty("version").GetString();
            this.stable = json.GetProperty("stable").GetBoolean();
        }

        public string GetVersion()
        {
            return this.version;
        }

        public bool IsStable()
        {
            return this.stable;
        }
    }

    public class MetaHandler
    {
        private readonly string metaPath;
        private List<GameVersion> versions;

        public MetaHandler(string path)
        {
            this.metaPath = path;
        }

        public void Load()
        {
            JsonDocument json = FabricService.QueryMetaJson(this.metaPath);
            this.versions = json.RootElement.EnumerateArray()
                .Select(item => new GameVersion(item))
                .ToList();
        }

        public IReadOnlyList<GameVersion> GetVersions()
        {
            return this.versions.AsReadOnly();
        }

        public GameVersion GetLatestVersion(bool snapshot)
        {
            if (this.versions.Count == 0)
                throw new InvalidOperationException("no versions available at " + this.metaPath);

            if (!snapshot)
            {
                foreach (GameVersion version in this.versions)
                {
                    if (version.IsStable())
                        return version;
                }
            }

            return this.versions[0];
        }
    }
}

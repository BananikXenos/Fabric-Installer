using AtlasClient.FabricInstallerUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasClient.FabricInstallerUtil
{
    public class Reference
    {
        public const string LOADER_NAME = "fabric-loader";
        public const string FABRIC_API_URL = "https://www.curseforge.com/minecraft/mc-mods/fabric-api/";
        public const string SERVER_LAUNCHER_URL = "https://fabricmc.net/use/server/";
        public const string MINECRAFT_LAUNCHER_MANIFEST = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
        public const string EXPERIMENTAL_LAUNCHER_MANIFEST = "https://maven.fabricmc.net/net/minecraft/experimental_versions.json";
        internal const string DEFAULT_META_SERVER = "https://meta.fabricmc.net/";
        internal const string DEFAULT_MAVEN_SERVER = "https://maven.fabricmc.net/";
        internal static readonly FabricService[] FabricServices = new FabricService[]
        {
            new FabricService("https://meta.fabricmc.net/", "https://maven.fabricmc.net/"),
            new FabricService("https://meta2.fabricmc.net/", "https://maven2.fabricmc.net/"),
            new FabricService("https://meta3.fabricmc.net/", "https://maven3.fabricmc.net/")
        };
    }
}

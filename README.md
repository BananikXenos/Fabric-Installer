
# Fabric Installer in C#

Simple Fabric Installer for C# ported from Java.\
It's basically the same installer without a gui and directly written in c#.\

**You don't have to credit me. You can use this code freely and edit it as you wish!**


## Features

- Get Game & Loader Versions List
- Get Latest Game & Loader Version
- Install fabric (duh)
- Installer progress logging


## Usage/Examples

```cs
// Load the Meta
FabricInstaller.GAME_VERSION_META.Load();
// Get latest game version that is not a snapshot
GameVersion latestGame = FabricInstaller.GAME_VERSION_META.GetLatestVersion(false);
Console.WriteLine(latestGame.GetVersion());

// Load the Meta
FabricInstaller.LOADER_META.Load();
// Get latest loader version that is not a snapshot
GameVersion latestLoader = FabricInstaller.LOADER_META.GetLatestVersion(false);
Console.WriteLine(latestLoader.GetVersion());

// Install latest fabric loader profile
FabricInstaller fabricInstaller = new FabricInstaller("Your path here", latestGame.GetVersion(), latestLoader.GetVersion());
// Add logging
fabricInstaller.OnPercentageChange += (percentage) =>
{
    Console.WriteLine(percentage + "%");
};
// Install
fabricInstaller.Install();
```


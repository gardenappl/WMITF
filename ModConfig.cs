using log4net;
using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace WMITF
{

    public class Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("$Mods.WMITF.DisplayWorldTooltips")]
		[Tooltip("$Mods.WMITF.DisplayWorldTooltips.Desc")]
		[DefaultValue(false)]
        public bool DisplayWorldTooltips;

        [Label("$Mods.WMITF.DisplayItemTooltips")]
		[Tooltip("$Mods.WMITF.DisplayItemTooltips.Desc")]
		[DefaultValue(true)]
        public bool DisplayItemTooltips;
		
        [Label("$Mods.WMITF.DisplayTechnicalNames")]
		[Tooltip("$Mods.WMITF.DisplayTechnicalNames.Desc")]
		[DefaultValue(false)]
        public bool DisplayTechnicalNames;

	}

    public class LegacyConfig
    {

        static bool DisplayWorldTooltips = false;
        const string DisplayWorldTooltipsKey = "DisplayWorldTooltips";
        static bool DisplayItemTooltips = true;
        const string DisplayItemTooltipsKey = "DisplayItemTooltips";
        static bool DisplayTechnicalNames = false;
        const string DisplayTechnicalNamesKey = "DisplayTechnicalNames";


        static string ConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "WMITF.json");
        static string OldConfigFolderPath = Path.Combine(Main.SavePath, "Mod Configs", "WMITF");
        static string OldConfigPath = Path.Combine(OldConfigFolderPath, "config.json");
        static string OldConfigVersionPath = Path.Combine(OldConfigFolderPath, "config.version");

        static readonly Preferences Settings = new Preferences(ConfigPath);

        //Make our own logger because it's too early in the mod load process to use VanillaTweaks.Instance
        static ILog Logger = LogManager.GetLogger("WMITFConfig");

        public static void Load()
        {
            if (Directory.Exists(OldConfigFolderPath))
            {
                if (File.Exists(OldConfigPath))
                {
                    if (File.Exists(ConfigPath))
                    {
                        Logger.Warn("Found config file in old folder! Deleting...");
                        File.Delete(OldConfigPath);
                    }
                    else
                    {
                        Logger.Warn("Found config file in old folder! Moving config...");
                        File.Move(OldConfigPath, ConfigPath);
                    }
                }
                if (File.Exists(OldConfigVersionPath))
                    File.Delete(OldConfigVersionPath);
                if (Directory.GetFiles(OldConfigFolderPath).Length == 0 && Directory.GetDirectories(OldConfigFolderPath).Length == 0)
                    Directory.Delete(OldConfigFolderPath);
                else
                    Logger.Warn("Old config folder still cotains some files/directories. They will not get deleted.");
            }

            if (!File.Exists(ConfigPath))
                return;

            if (!ReadConfig())
            {
                Logger.Warn("Failed to read legacy config file! Deleting...");
                File.Delete(ConfigPath);
                return;
            }
            MoveToNewFormat();
        }

        public static bool ReadConfig()
        {
            if (Settings.Load())
            {
                Settings.Get(DisplayWorldTooltipsKey, ref DisplayWorldTooltips);
                Settings.Get(DisplayItemTooltipsKey, ref DisplayItemTooltips);
                Settings.Get(DisplayTechnicalNamesKey, ref DisplayTechnicalNames);
                return true;
            }
            return false;
        }

        static void MoveToNewFormat()
        {
            Logger.Warn("Migrating to new format...");

            var newConfigPath = Path.Combine(ConfigManager.ModConfigPath,
                    nameof(WMITF) + "_Config" + ".json");
            var newConfig = new Preferences(newConfigPath);
            newConfig.Put(DisplayWorldTooltipsKey, DisplayWorldTooltips);
            newConfig.Put(DisplayItemTooltipsKey, DisplayItemTooltips);
            newConfig.Put(DisplayTechnicalNamesKey, DisplayTechnicalNames);
            newConfig.Save();

            if (!File.Exists(newConfigPath))
                File.Move(ConfigPath, newConfigPath);
            else
                File.Delete(ConfigPath);
        }
    }
}



using System;
using Terraria.ModLoader;

namespace WMITF
{
    public class WMITF : Mod
    {
        static public ModKeybind ToggleTooltipsHotkey;
        static public ModKeybind TechnicalNamesHotkey;

        public WMITF()
        {
            LegacyConfig.Load();
        }

        public override void Load()
        {
            ToggleTooltipsHotkey = KeybindLoader.RegisterKeybind(this, "Tile/NPC Mod Tooltip", "OemQuestion");
            TechnicalNamesHotkey = KeybindLoader.RegisterKeybind(this, "Technical Names", "N");
        }

        public override void Unload()
        {
            ToggleTooltipsHotkey = null;
            TechnicalNamesHotkey = null;
        }

		public static bool CheckAprilFools()
		{
			var date = DateTime.Now;
			return date.Month == 4 && date.Day <= 2;
		}

		// Hamstar's Mod Helpers integration
		public static string GithubUserName { get { return "goldenapple3"; } }
		public static string GithubProjectName { get { return "WMITF"; } }
	}
}

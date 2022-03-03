
using System;
using Terraria;
using Terraria.ModLoader;

namespace WMITF
{
    public class WMITF : Mod
    {
        static public ModKeybind ToggleTooltipsHotkey;
        static public ModKeybind TechnicalNamesHotkey;
        static public int unloadedItemType;
        static public int aprilFoolsItemType;
        static public int unloadedTileType1;
        static public int unloadedTileType2;
        static public int unloadedTileType3;

        public WMITF()
        {
            LegacyConfig.Load();
        }

        public override void Load()
        {
            ToggleTooltipsHotkey = KeybindLoader.RegisterKeybind(this, "Tile/NPC Mod Tooltip", "OemQuestion");
            TechnicalNamesHotkey = KeybindLoader.RegisterKeybind(this, "Technical Names", "N");
        }

        public override void PostSetupContent()
        {
            bool success;
            ModLoader.TryGetMod("ModLoader", out Mod modLoaderMod);
            success = modLoaderMod.TryFind<ModItem>("UnloadedItem", out ModItem unloadedItem);
            if (success)
                unloadedItemType = unloadedItem.Type;
            success = modLoaderMod.TryFind<ModItem>("AprilFools", out ModItem aprilFoolsItem);
            if (success)
                aprilFoolsItemType = aprilFoolsItem.Type;
            success = modLoaderMod.TryFind<ModTile>("UnloadedSolidTile", out ModTile unloadedTile1);
            if (success)
                unloadedTileType1 = unloadedTile1.Type;
            success = modLoaderMod.TryFind<ModTile>("UnloadedNonSolidTile", out ModTile unloadedTile2);
            if (success)
                unloadedTileType2 = unloadedTile2.Type;
            success = modLoaderMod.TryFind<ModTile>("UnloadedSemiSolidTile", out ModTile unloadedTile3);
            if (success)
                unloadedTileType3 = unloadedTile3.Type;
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

        public static bool IsUnloadedTile(Tile tile)
        {
            if (tile.HasTile)
            {
                if (tile.TileType == unloadedTileType1 || tile.TileType == unloadedTileType2 || tile.TileType == unloadedTileType3)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static void Log(object message)
        {
            ModContent.GetInstance<WMITF>().Logger.Info(message);
        }

        // Hamstar's Mod Helpers integration
        public static string GithubUserName { get { return "goldenapple3"; } }
		public static string GithubProjectName { get { return "WMITF"; } }
	}
}

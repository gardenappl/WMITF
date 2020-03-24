
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace WMITF
{
    public class WMITF : Mod
    {
        static string MouseText;
        static bool SecondLine;
        static ModHotKey ToggleTooltipsHotkey;
        static ModHotKey TechnicalNamesHotkey;
        static bool DisplayWorldTooltips = false;
        static bool DisplayItemTooltips = true;
        static bool DisplayTechnicalNames = false;

        static Preferences Configuration = new Preferences(Path.Combine(Main.SavePath, "Mod Configs", "WMITF.json"));

        public override void Load()
        {
            ToggleTooltipsHotkey = RegisterHotKey("Tile/NPC Mod Tooltip", "OemQuestion");
            TechnicalNamesHotkey = RegisterHotKey("Technical Names", "N");
            if (!ReadConfig())
            {
                SetConfigDefaults();
                SaveConfig();
            }
            Configuration.AutoSave = true;
        }

        public override void Unload()
        {
            ToggleTooltipsHotkey = null;
            TechnicalNamesHotkey = null;
        }


		#region Config

		static bool ReadConfig()
		{
			if(Configuration.Load())
			{
				Configuration.Get("DisplayWorldTooltips", ref DisplayWorldTooltips);
				Configuration.Get("DisplayItemTooltips", ref DisplayItemTooltips);
				Configuration.Get("DisplayTechnicalNames", ref DisplayTechnicalNames);
				return true;
			}
			return false;
		}

		static void SetConfigDefaults()
		{
			DisplayWorldTooltips = false;
			DisplayItemTooltips = true;
			DisplayTechnicalNames = false;
		}

		static void SaveConfig()
		{
			Configuration.Put("DisplayWorldTooltips", DisplayWorldTooltips);
			Configuration.Put("DisplayItemTooltips", DisplayItemTooltips);
			Configuration.Put("DisplayTechnicalNames", DisplayTechnicalNames);
			Configuration.Save();
		}

		#endregion Config

		public static bool CheckAprilFools()
		{
			var date = DateTime.Now;
			return date.Month == 4 && date.Day <= 2;
		}

		#region In-World Tooltips

		public class WorldTooltips : ModPlayer
		{
			public override void ProcessTriggers(TriggersSet triggersSet)
			{
				if(ToggleTooltipsHotkey.JustPressed)
				{
					if(DisplayWorldTooltips)
					{
						DisplayWorldTooltips = false;
						Main.NewText(Language.GetTextValue("Mods.WMITF.WorldTooltipsOff"));
					}
					else
					{
						DisplayWorldTooltips = true;
						Main.NewText(Language.GetTextValue("Mods.WMITF.WorldTooltipsOn"));
					}
					Configuration.Put("DisplayWorldTooltips", DisplayWorldTooltips);
				}
				if(TechnicalNamesHotkey.JustPressed)
				{
					if(DisplayTechnicalNames)
					{
						DisplayTechnicalNames = false;
						Main.NewText(Language.GetTextValue("Mods.WMITF.TechNamesOff"));
					}
					else
					{
						DisplayTechnicalNames = true;
						Main.NewText(Language.GetTextValue("Mods.WMITF.TechNamesOn"));
					}
					Configuration.Put("DisplayTechnicalNames", DisplayTechnicalNames);
				}
			}
			
			public override void PostUpdate()
			{
				if(Main.dedServ || !DisplayWorldTooltips)
					return;
				MouseText = String.Empty;
				SecondLine = false;
				var modLoaderMod = ModLoader.GetMod("ModLoader"); //modmodloadermodmodloadermodmodloader
				int mysteryTile = modLoaderMod.TileType("MysteryTile");
				int mysteryTile2 = modLoaderMod.TileType("PendingMysteryTile");
				
				var tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
				if(tile != null)
				{
					if(tile.active() && tile.type != mysteryTile && tile.type != mysteryTile2)
					{
						var modTile = TileLoader.GetTile(tile.type);
						if(modTile != null)
						{
							MouseText = DisplayTechnicalNames ? (modTile.mod.Name + ":" + modTile.Name) : modTile.mod.DisplayName;
						}
					}
					else
					{
						var modWall = WallLoader.GetWall(tile.wall);
						if(modWall != null)
						{
							MouseText = DisplayTechnicalNames ? (modWall.mod.Name + ":" + modWall.Name) : modWall.mod.DisplayName;
						}
					}
				}
				
				var mousePos = Main.MouseWorld;
				for(int i = 0; i < Main.maxNPCs; i++)
				{
					var npc = Main.npc[i];
					if(mousePos.Between(npc.TopLeft, npc.BottomRight))
					{
						var modNPC = NPCLoader.GetNPC(npc.type);
						if(modNPC != null && npc.active && !NPCID.Sets.ProjectileNPC[npc.type])
						{
							MouseText = DisplayTechnicalNames ? (modNPC.mod.Name + ":" + modNPC.Name) : modNPC.mod.DisplayName;
							SecondLine = true;
							break;
						}
					}
				}
				if(MouseText != String.Empty && Main.mouseText)
				{
					SecondLine = true;
				}
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if(index != -1)
			{
				//Thank you jopojelly! (taken from https://github.com/JavidPack/SummonersAssociation)
				layers.Insert(index, new LegacyGameInterfaceLayer("WMITF: Mouse Text", delegate
				{
					if(DisplayWorldTooltips && !String.IsNullOrEmpty(MouseText))
					{
						string coloredString = String.Format("[c/{1}:[{0}][c/{1}:]]", MouseText, Colors.RarityBlue.Hex3());
						var text = ChatManager.ParseMessage(coloredString, Color.White).ToArray();
						//float x = Main.fontMouseText.MeasureString(MouseText).X;
						float x = ChatManager.GetStringSize(Main.fontMouseText, text, Vector2.One).X;
						var pos = Main.MouseScreen + new Vector2(16f, 16f);
						if(pos.Y > (float)(Main.screenHeight - 30))
							pos.Y = (float)(Main.screenHeight - 30);
						if(pos.X > (float)(Main.screenWidth - x))
							pos.X = (float)(Main.screenWidth - x);
						if(SecondLine)
							pos.Y += Main.fontMouseText.LineSpacing;
						int hoveredSnippet;
						ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, pos, 0f, Vector2.Zero, Vector2.One, out hoveredSnippet);
					}
					return true;
				}, InterfaceScaleType.UI));
			}
		}

		#endregion

		#region Item Tooltips

		public class ItemTooltips : GlobalItem
		{
			public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
			{
				var modLoaderMod = ModLoader.GetMod("ModLoader");
				int mysteryItem = modLoaderMod.ItemType("MysteryItem");
				int aprilFoolsItem = modLoaderMod.ItemType("AprilFools");
				if(DisplayItemTooltips && item.type != mysteryItem && (item.type != aprilFoolsItem || !CheckAprilFools()))
				{
					if(item.modItem != null && !item.Name.Contains("[" + item.modItem.mod.Name + "]") && !item.Name.Contains("[" + item.modItem.mod.DisplayName + "]"))
					{
						string text = DisplayTechnicalNames ? (item.modItem.mod.Name + ":" + item.modItem.Name) : item.modItem.mod.DisplayName;
						var line = new TooltipLine(mod, mod.Name, "[" + text + "]");
						line.overrideColor = Colors.RarityBlue;
						tooltips.Add(line);
					}
				}
			}
		}

		#endregion

		#region Hamstar's Mod Helpers integration

		public static string GithubUserName { get { return "goldenapple3"; } }
		public static string GithubProjectName { get { return "WMITF"; } }

		public static string ConfigFileRelativePath { get { return "Mod Configs/WMITF.json"; } }

		public static void ReloadConfigFromFile()
		{
			ReadConfig();
		}

		public static void ResetConfigFromDefaults()
		{
			SetConfigDefaults();
			SaveConfig();
		}

		#endregion
	}
}

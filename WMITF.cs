
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

namespace WMITF
{
	public class WMITF : Mod
	{
		static string MouseText;
		static bool SecondLine;
		static ModHotKey ToggleTooltipsHotkey;
		static bool DisplayWorldTooltips = false;
		static bool DisplayItemTooltips = true;
		
		static Preferences Configuration = new Preferences(Path.Combine(Main.SavePath, "Mod Configs", "WMITF.json"));
		
		public override void Load()
		{
			ToggleTooltipsHotkey = RegisterHotKey("Tile/NPC Mod Tooltip", "OemQuestion");
			if(Configuration.Load())
			{
				Configuration.Get("DisplayWorldTooltips", ref DisplayWorldTooltips);
				Configuration.Get("DisplayItemTooltips", ref DisplayItemTooltips);
			}
			else
			{
				Configuration.Put("DisplayWorldTooltips", DisplayWorldTooltips);
				Configuration.Put("DisplayItemTooltips", DisplayItemTooltips);
				Configuration.Save();
			}
			Configuration.AutoSave = true;
		}
		
		public static bool CheckAprilFools()
		{
			DateTime now = DateTime.Now;
			return now.Month == 4 && now.Day <= 2;
		}
		
		public class WorldTooltips : ModPlayer
		{
			public override void ProcessTriggers(TriggersSet triggersSet)
			{
				if(ToggleTooltipsHotkey.JustPressed)
				{
					DisplayWorldTooltips = !DisplayWorldTooltips;
					Configuration.Put("DisplayWorldTooltips", DisplayWorldTooltips);
					Main.PlaySound(SoundID.MenuTick);
				}
			}
			
			public override void PostUpdate()
			{
				if(Main.dedServ || !DisplayWorldTooltips) return;
				Mod displayMod = null;
				Mod modLoaderMod = ModLoader.GetMod("ModLoader"); //modmodloadermodmodloadermodmodloader
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
							displayMod = modTile.mod;
						}
					}
					else
					{
						var modWall = WallLoader.GetWall(tile.wall);
						if(modWall != null)
						{
							displayMod = modWall.mod;
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
							displayMod = modNPC.mod;
							SecondLine = true;
							break;
						}
					}
				}
				if(displayMod != null)
				{
					MouseText = String.Format("[c/{1}:[{0}][c/{1}:]]", displayMod.DisplayName, Colors.RarityBlue.Hex3());
					if(Main.mouseText)
					{
						SecondLine = true;
					}
				}
			}
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1) {
                layers.Insert(index, new GameInterfaceLayer("WMITF: Mouse Text", InterfaceScaleType.UI));
            }
        }

        //Thank you jopojelly! (taken from https://github.com/JavidPack/SummonersAssociation)
        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            base.PostDrawInterface(spriteBatch);
            if (DisplayWorldTooltips && !String.IsNullOrEmpty(MouseText)) {
                float x = Main.fontMouseText.MeasureString(MouseText).X;
                var pos = Main.MouseScreen + new Vector2(16f, 16f);
                if (pos.Y > (float)(Main.screenHeight - 30)) {
                    pos.Y = (float)(Main.screenHeight - 30);
                }
                if (pos.X > (float)(Main.screenWidth - x)) {
                    pos.X = (float)(Main.screenWidth - x);
                }
                if (SecondLine) {
                    pos.Y += Main.fontMouseText.LineSpacing;
                }
                int hoveredSnippet;
                var array = ChatManager.ParseMessage(MouseText, Color.White);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, array.ToArray(), pos, 0f, Vector2.Zero, Vector2.One, out hoveredSnippet);

                SecondLine = false;
                MouseText = "";
            }
        }

        public class ItemTooltips : GlobalItem
		{
			public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
			{
				Mod modLoaderMod = ModLoader.GetMod("ModLoader");
				int mysteryItem = modLoaderMod.ItemType("MysteryItem");
				int aprilFoolsItem = modLoaderMod.ItemType("AprilFools");
				if(DisplayItemTooltips && item.type != mysteryItem && (item.type != aprilFoolsItem || CheckAprilFools()))
				{
					if(item.modItem != null && !item.Name.Contains("[" + item.modItem.mod.Name + "]") && !item.Name.Contains("[" + item.modItem.mod.DisplayName + "]"))
					{
						var line = new TooltipLine(mod, mod.Name, "[" + item.modItem.mod.DisplayName + "]");
						line.overrideColor = Colors.RarityBlue;
						tooltips.Add(line);
					}
				}
			}
		}
	}
}

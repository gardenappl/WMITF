
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace WMITF
{
	public class WMITF : Mod
	{
		static string MouseText;
		static bool SecondLine;
		static ModHotKey ToggleTooltipsHotkey;
		static bool DisplayTooltips = true;
		
		public WMITF()
		{
			Properties = new ModProperties
			{
				Autoload = true
			};
		}
		
		public override void Load()
		{
			ToggleTooltipsHotkey = RegisterHotKey("Tile/NPC Mod Tooltip", "OemQuestion");
		}
		
		public class WorldTooltips : ModPlayer
		{
			public override void ProcessTriggers(TriggersSet triggersSet)
			{
				if(ToggleTooltipsHotkey.JustPressed)
				{
					DisplayTooltips = !DisplayTooltips;
					Main.PlaySound(SoundID.MenuTick);
				}
			}
			
			public override void PostUpdate()
			{
				if(Main.dedServ || !DisplayTooltips) return;
				Mod displayMod = null;
				
				var tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
				if(tile != null)
				{
					if(tile.active())
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
		
		public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
			int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if(index != -1)
			{
				layers.Insert(index, new MethodSequenceListItem("WMITF: Mouse Text", DrawMouseText));
			}
		}
		
		//Thank you jopojelly! (taken from https://github.com/JavidPack/SummonersAssociation)
		bool DrawMouseText()
		{
			if(DisplayTooltips && !String.IsNullOrEmpty(MouseText))
			{
				float x = Main.fontMouseText.MeasureString(MouseText).X;
				var pos = Main.MouseScreen + new Vector2(16f, 16f);
				if (pos.Y > (float)(Main.screenHeight - 30))
				{
					pos.Y = (float)(Main.screenHeight - 30);
				}
				if (pos.X > (float)(Main.screenWidth - x))
				{
					pos.X = (float)(Main.screenWidth - x);
				}
				if(SecondLine)
				{
					pos.Y += Main.fontMouseText.LineSpacing;
				}
				int hoveredSnippet;
				var array = ChatManager.ParseMessage(MouseText, Color.White);
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, array, pos, 0f, Vector2.Zero, Vector2.One, out hoveredSnippet);
				
				SecondLine = false;
				MouseText = "";
			}
			return true;
		}
		
		public class ItemTooltips : GlobalItem
		{
			public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
			{
				if(item.modItem != null && !item.name.Contains("[" + item.modItem.mod.Name + "]") && !item.name.Contains("[" + item.modItem.mod.DisplayName + "]"))
				{
					var line = new TooltipLine(mod, mod.Name, "[" + item.modItem.mod.DisplayName + "]");
					line.overrideColor = Colors.RarityBlue;
					tooltips.Add(line);
				}
			}
		}
	}
}

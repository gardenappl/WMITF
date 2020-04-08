
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace WMITF
{
    public class WMITF : Mod
    {
        static public string MouseText;
        static public bool SecondLine;
        static public ModHotKey ToggleTooltipsHotkey;
        static public ModHotKey TechnicalNamesHotkey;

        public WMITF()
        {
            LegacyConfig.Load();
        }

        public override void Load()
        {
            ToggleTooltipsHotkey = RegisterHotKey("Tile/NPC Mod Tooltip", "OemQuestion");
            TechnicalNamesHotkey = RegisterHotKey("Technical Names", "N");
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

        //Setup for drawing in world tooltips
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if(index != -1)
			{
				//Thank you jopojelly! (taken from https://github.com/JavidPack/SummonersAssociation)
				layers.Insert(index, new LegacyGameInterfaceLayer("WMITF: Mouse Text", delegate
				{
					if(ModContent.GetInstance<Config>().DisplayWorldTooltips && !String.IsNullOrEmpty(MouseText))
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



		// Hamstar's Mod Helpers integration
		public static string GithubUserName { get { return "goldenapple3"; } }
		public static string GithubProjectName { get { return "WMITF"; } }
	}
}

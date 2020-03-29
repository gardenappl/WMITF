
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace WMITF
{
    public class WorldTooltips : ModPlayer
    {

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (WMITF.ToggleTooltipsHotkey.JustPressed)
            {
                if (ModContent.GetInstance<Config>().DisplayWorldTooltips)
                {
                    ModContent.GetInstance<Config>().DisplayWorldTooltips = false;
                    Main.NewText(Language.GetTextValue("Mods.WMITF.WorldTooltipsOff"));
                }
                else
                {
                    ModContent.GetInstance<Config>().DisplayWorldTooltips = true;
                    Main.NewText(Language.GetTextValue("Mods.WMITF.WorldTooltipsOn"));
                }
            }
            if (WMITF.TechnicalNamesHotkey.JustPressed)
            {
                if (ModContent.GetInstance<Config>().DisplayTechnicalNames)
                {
                    ModContent.GetInstance<Config>().DisplayTechnicalNames = false;
                    Main.NewText(Language.GetTextValue("Mods.WMITF.TechNamesOff"));
                }
                else
                {
                    ModContent.GetInstance<Config>().DisplayTechnicalNames = true;
                    Main.NewText(Language.GetTextValue("Mods.WMITF.TechNamesOn"));
                }
            }
        }

        public override void PostUpdate()
        {
            if (Main.dedServ || !ModContent.GetInstance<Config>().DisplayWorldTooltips)
                return;
            WMITF.MouseText = String.Empty;
            WMITF.SecondLine = false;
            var modLoaderMod = ModLoader.GetMod("ModLoader"); //modmodloadermodmodloadermodmodloader
            int mysteryTile = modLoaderMod.TileType("MysteryTile");
            int mysteryTile2 = modLoaderMod.TileType("PendingMysteryTile");

            var tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if (tile != null)
            {
                if (tile.active() && tile.type != mysteryTile && tile.type != mysteryTile2)
                {
                    var modTile = TileLoader.GetTile(tile.type);
                    if (modTile != null)
                    {
                        WMITF.MouseText = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (modTile.mod.Name + ":" + modTile.Name) : modTile.mod.DisplayName;
                    }
                }
                else
                {
                    var modWall = WallLoader.GetWall(tile.wall);
                    if (modWall != null)
                    {
                        WMITF.MouseText = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (modWall.mod.Name + ":" + modWall.Name) : modWall.mod.DisplayName;
                    }
                }
            }

            var mousePos = Main.MouseWorld;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (mousePos.Between(npc.TopLeft, npc.BottomRight))
                {
                    var modNPC = NPCLoader.GetNPC(npc.type);
                    if (modNPC != null && npc.active && !NPCID.Sets.ProjectileNPC[npc.type])
                    {
                        WMITF.MouseText = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (modNPC.mod.Name + ":" + modNPC.Name) : modNPC.mod.DisplayName;
                        WMITF.SecondLine = true;
                        break;
                    }
                }
            }
            if (WMITF.MouseText != String.Empty && Main.mouseText)
            {
                WMITF.SecondLine = true;
            }
        }
    }
}

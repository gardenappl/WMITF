
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
            WMITFModSystem.MouseText = String.Empty;
            WMITFModSystem.SecondLine = false;
            var tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if (tile != null)
            {
                if (tile.IsActive)
                {
                    var modTile = TileLoader.GetTile(tile.type);
                    if (modTile != null)
                    {
                        WMITFModSystem.MouseText = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (modTile.Mod.Name + ":" + modTile.Name) : modTile.Mod.DisplayName;
                    }
                }
                else
                {
                    var modWall = WallLoader.GetWall(tile.wall);
                    if (modWall != null)
                    {
                        WMITFModSystem.MouseText = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (modWall.Mod.Name + ":" + modWall.Name) : modWall.Mod.DisplayName;
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
                        WMITFModSystem.MouseText = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (modNPC.Mod.Name + ":" + modNPC.Name) : modNPC.Mod.DisplayName;
                        WMITFModSystem.SecondLine = true;
                        break;
                    }
                }
            }
            if (WMITFModSystem.MouseText != String.Empty && Main.mouseText)
            {
                WMITFModSystem.SecondLine = true;
            }
        }
    }
}

using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WMITF
{
    public class ItemTooltips : GlobalItem
		{
			public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
			{
				var modLoaderMod = ModLoader.GetMod("ModLoader");
				int mysteryItem = modLoaderMod.ItemType("MysteryItem");
				int aprilFoolsItem = modLoaderMod.ItemType("AprilFools");
				if(ModContent.GetInstance<Config>().DisplayItemTooltips && item.type != mysteryItem && (item.type != aprilFoolsItem || !WMITF.CheckAprilFools()))
				{
					if(item.modItem != null && !item.Name.Contains("[" + item.modItem.mod.Name + "]") && !item.Name.Contains("[" + item.modItem.mod.DisplayName + "]"))
					{
						string text = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (item.modItem.mod.Name + ":" + item.modItem.Name) : item.modItem.mod.DisplayName;
						var line = new TooltipLine(mod, mod.Name, "[" + text + "]");
						line.overrideColor = Colors.RarityBlue;
						tooltips.Add(line);
					}
				}
			}
		}
}

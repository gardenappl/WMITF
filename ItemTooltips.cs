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

				if (ModContent.GetInstance<Config>().DisplayItemTooltips && item.type != WMITF.unloadedItemType && (item.type != WMITF.aprilFoolsItemType || !WMITF.CheckAprilFools()))
				{
					if(item.ModItem != null && !item.Name.Contains("[" + item.ModItem.Mod.Name + "]") && !item.Name.Contains("[" + item.ModItem.Mod.DisplayName + "]"))
					{
						string text = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (item.ModItem.Mod.Name + ":" + item.ModItem.Name) : item.ModItem.Mod.DisplayName;
						TooltipLine line = new (Mod, Mod.Name, "[" + text + "]");
						line.overrideColor = Colors.RarityBlue;
						tooltips.Add(line);
					}
				}
			}
		}
}

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
				ModLoader.TryGetMod("ModLoader", out Mod modLoaderMod);
				modLoaderMod.TryFind<ModItem>("AprilFools", out ModItem aprilFoolsItem);
				if(ModContent.GetInstance<Config>().DisplayItemTooltips && (item.type != aprilFoolsItem.Type || !WMITF.CheckAprilFools()))
				{
					if(item.ModItem != null && !item.Name.Contains("[" + item.ModItem.Mod.Name + "]") && !item.Name.Contains("[" + item.ModItem.Mod.DisplayName + "]"))
					{
						string text = ModContent.GetInstance<Config>().DisplayTechnicalNames ? (item.ModItem.Mod.Name + ":" + item.ModItem.Name) : item.ModItem.Mod.DisplayName;
						var line = new TooltipLine(Mod, Mod.Name, "[" + text + "]");
						line.overrideColor = Colors.RarityBlue;
						tooltips.Add(line);
					}
				}
			}
		}
}

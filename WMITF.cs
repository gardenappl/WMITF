
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WMITF
{
	public class WMITF : Mod
	{
		public WMITF()
		{
			Properties = new ModProperties
			{
				Autoload = true
			};
		}
		
		public class TooltipTweak : GlobalItem
		{
			public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
			{
				if(item.modItem != null && !item.name.Contains("[" + item.modItem.mod.Name + "]") && !item.name.Contains("[" + item.modItem.mod.DisplayName + "]"))
				{
					var line = new TooltipLine(mod, "WhichMod", "[" + item.modItem.mod.DisplayName + "]");
					line.overrideColor = Colors.RarityBlue;
					tooltips.Add(line);
				}
			}
		}
	}
}

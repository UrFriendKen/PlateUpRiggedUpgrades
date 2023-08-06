using Kitchen.Modules;
using KitchenData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KitchenRiggedUpgrades.Grids
{
    public class GridMenuApplianceConfig : GridMenuConfig
    {
        public override GridMenu Instantiate(Transform container, int player, bool has_back)
        {
            return new ApplianceGridMenu(new List<GridItemAppliance>(), container, player, has_back);
        }

        public virtual ApplianceGridMenu Instantiate(Appliance appliance, Action<int> callback, Transform container, int player, bool has_back)
        {
            if (!(appliance?.HasUpgrades ?? false))
                new ApplianceGridMenu(new List<GridItemAppliance>(), container, player, has_back);

            List<GridItemAppliance> gridAppliances = new List<GridItemAppliance>()
            {
                new GridItemAppliance(0, callback)
            };
            gridAppliances.AddRange(appliance.Upgrades.Select(upgrade => new GridItemAppliance(upgrade, callback)));
            return new ApplianceGridMenu(gridAppliances, container, player, has_back);
        }
    }
}

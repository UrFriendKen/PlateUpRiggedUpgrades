using Kitchen.Modules;
using KitchenData;
using KitchenRiggedUpgrades.Utils;
using ModdedCosmeticsIntegration.Grids;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenRiggedUpgrades.Grids
{
    public class ApplianceGridMenu : CustomGridMenu<GridItemAppliance>
    {
        public ApplianceGridMenu(List<GridItemAppliance> items, Transform container, int player, bool has_back) : base(items, container, player, has_back)
        {
        }

        protected override void SetupElement(GridItemAppliance item, GridMenuElement element)
        {
            element.Set(item);
        }

        protected override void OnSelect(GridItemAppliance item)
        {
            item.DoCallback();
        }
    }

    [Serializable]
    public struct GridItemAppliance : IGridItem
    {
        public readonly int ApplianceID;
        public readonly Appliance Appliance;
        private Action<int> SelectCallback;

        public GridItemAppliance(int applianceID, Action<int> callback)
        {
            ApplianceID = applianceID;
            GameData.Main.TryGet(applianceID, out Appliance);
            SelectCallback = callback;
        }

        public GridItemAppliance(Appliance appliance, Action<int> callback)
        {
            ApplianceID = appliance?.ID ?? 0;
            Appliance = appliance;
            SelectCallback = callback;
        }

        public int SnapshotKey => ApplianceID;

        public Texture2D GetSnapshot()
        {
            if (ApplianceID == 0)
            {
                int textureSize = 512;
                Texture2D texture = new Texture2D(textureSize, textureSize);
                Color transparent = new Color(0f, 0f, 0f, 0f);
                int crossSize = 150;
                int lineWidth = 30;

                int majorDistance = Mathf.FloorToInt(Mathf.Sqrt(2 * lineWidth * lineWidth));

                crossSize = Mathf.Clamp(crossSize, 0, 512);
                int padding = (textureSize - crossSize) / 2;
                for (int x = 0; x < textureSize; x++)
                {
                    for (int y = 0; y < textureSize; y++)
                    {
                        if (x >= padding && x < padding + crossSize &&
                            y >= padding && y < padding + crossSize)
                        {
                            if (IsWhite())
                            {
                                texture.SetPixel(x, y, Color.white);
                                continue;
                            }

                            bool IsWhite()
                            {
                                return (x + majorDistance > y && x - majorDistance < y) ||
                                    (-x + majorDistance + textureSize > y && -x - majorDistance + textureSize < y);
                            }
                        }
                        texture.SetPixel(x, y, transparent);
                    }
                }
                texture.Apply();
                return texture;
            }
            return SnapshotUtils.GetApplianceSnapshot(Appliance.Prefab);
        }

        public void DoCallback()
        {
            SelectCallback?.Invoke(ApplianceID);
        }
    }
}

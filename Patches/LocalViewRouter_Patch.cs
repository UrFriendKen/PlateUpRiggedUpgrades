using HarmonyLib;
using Kitchen;
using KitchenRiggedUpgrades.Grids;
using System.Reflection;
using UnityEngine;

namespace KitchenRiggedUpgrades.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        static MethodInfo m_GetPrefab = typeof(LocalViewRouter).GetMethod("GetPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo f_Container = typeof(CostumeChangeIndicator).GetField("Container", BindingFlags.NonPublic | BindingFlags.Instance);

        static GameObject _container;
        static GameObject _upgradeSelectorPrefab;

        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPrefix]
        static bool GetPrefab_Prefix(ref LocalViewRouter __instance, ViewType view_type, ref GameObject __result)
        {
            if (view_type == Main.UpgradeSelectorViewType)
            {
                if (_container == null)
                {
                    _container = new GameObject("Rigged Upgrades Container");
                    _container.SetActive(false);
                }

                if (_upgradeSelectorPrefab == null)
                {
                    object obj = m_GetPrefab.Invoke(__instance, new object[] { ViewType.CostumeChangeInfo });
                    if (obj != null)
                    {
                        _upgradeSelectorPrefab = GameObject.Instantiate((GameObject)obj);
                        _upgradeSelectorPrefab.transform.SetParent(_container.transform, false);
                        CostumeChangeIndicator costumeChangeIndicator = _upgradeSelectorPrefab.GetComponent<CostumeChangeIndicator>();
                        if (costumeChangeIndicator != null)
                        {
                            UpgradeIndicator upgradeIndicator = _upgradeSelectorPrefab.AddComponent<UpgradeIndicator>();
                            upgradeIndicator.Container = (Transform)f_Container?.GetValue(costumeChangeIndicator);
                            upgradeIndicator.RootMenuConfig = new GridMenuApplianceConfig();
                            Component.DestroyImmediate(costumeChangeIndicator);
                        }
                    }
                }
                __result = _upgradeSelectorPrefab;
                return false;
            }
            return true;
        }
    }
}

using HarmonyLib;
using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenRiggedUpgrades.Patches
{
    [HarmonyPatch]
    static class ActivatePlayerSpecificUIIndicators_Patch
    {
        [HarmonyPatch(typeof(ActivatePlayerSpecificUIIndicators), "IsPossible")]
        [HarmonyPrefix]
        static bool IsPossible_Prefix(ref InteractionData data, ref bool __result)
        {
            if (data.Context.Has<CCustomTriggerPlayerSpecificUI>(data.Target))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}

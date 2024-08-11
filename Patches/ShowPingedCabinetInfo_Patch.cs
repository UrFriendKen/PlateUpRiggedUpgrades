using HarmonyLib;
using Kitchen;

namespace KitchenRiggedUpgrades.Patches
{
    [HarmonyPatch]
    static class ShowPingedCabinetInfo_Patch
    {
        [HarmonyPatch(typeof(ShowPingedCabinetInfo), "IsPossible")]
        [HarmonyPrefix]
        static bool IsPossible_Prefix(ref InteractionData data, ref bool __result)
        {
            if (data.Context.Require(data.Target, out CTriggerPlayerSpecificUI trigger) &&
                trigger.IsTriggered)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}

using HarmonyLib;
using Kitchen;
using System;
using System.Reflection;

namespace KitchenRiggedUpgrades.Patches
{
    [HarmonyPatch]
    static class ImproveBlueprintAfterDuration_Patch
    {
        static readonly Type TARGET_TYPE = typeof(ImproveBlueprintAfterDuration);
        const bool IS_ORIGINAL_LAMBDA_BODY = true;
        const int LAMBDA_BODY_INDEX = 0;
        const string TARGET_METHOD_NAME = "";

        public static MethodBase TargetMethod()
        {
            Type type = IS_ORIGINAL_LAMBDA_BODY ? AccessTools.FirstInner(TARGET_TYPE, t => t.Name.Contains($"c__DisplayClass_OnUpdate_LambdaJob{LAMBDA_BODY_INDEX}")) : TARGET_TYPE;
            return AccessTools.FirstMethod(type, method => method.Name.Contains(IS_ORIGINAL_LAMBDA_BODY ? "OriginalLambdaBody" : TARGET_METHOD_NAME));
        }

        [HarmonyPrefix]
        static bool OriginalLambdaBody_Prefix(ref CDeskTarget target, ref CTakesDuration duration, in CModifyBlueprintStoreAfterDuration improvement, ref float ___discount)
        {
            if (!duration.Active || duration.Remaining > 0f || !improvement.PerformUpgrade || !PatchController.TryUpgrade(target, ___discount))
	{
                return true;
            };
            return false;
        }
    }
}

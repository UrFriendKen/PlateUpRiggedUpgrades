using HarmonyLib;
using Kitchen;
using MessagePack;
using System;
using System.Reflection;
using Unity.Entities;

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
        static bool OriginalLambdaBody_Prefix(ref Entity desk, ref CDeskTarget target, ref CTakesDuration duration, ref float ___discount)
        {
            if (!duration.Active ||
                duration.Remaining > 0f ||
                !PatchController.StaticRequire(desk, out CModifyBlueprintStoreAfterDuration improvement) ||
                !improvement.PerformUpgrade ||
                !PatchController.TryUpgrade(target, ___discount))
	{
                return true;
            };
            return false;
        }
    }
}

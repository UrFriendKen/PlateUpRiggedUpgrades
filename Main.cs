using Kitchen;
using KitchenData;
using KitchenRiggedUpgrades.Utils;
using System.Reflection;
using UnityEngine;

namespace KitchenRiggedUpgrades
{
    public class Main : BaseMain
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "Rigged Upgrades";
        public const string MOD_VERSION = "0.1.0";

        internal static ViewType UpgradeSelectorViewType = (ViewType)HashUtils.GetInt32HashCode($"{MOD_GUID}:UpgradeSelector");

        public Main() : base(MOD_GUID, MOD_NAME, MOD_VERSION, Assembly.GetExecutingAssembly())
        {
        }

        public override void OnPostActivate(KitchenMods.Mod mod)
        {
        }

        public override void PreInject()
        {
            if (GameData.Main.TryGet(-571205127, out Appliance blueprintCabinet) &&
                blueprintCabinet.Prefab != null && blueprintCabinet.Prefab.GetComponent<PreferredUpgradeView>() == null)
            {
                GameObject container = new GameObject("PreferredUpgrade");
                container.transform.SetParent(blueprintCabinet.Prefab.transform);
                container.transform.localPosition = new Vector3(0f, 1.3f, 0.25f);
                container.transform.localRotation = Quaternion.identity;
                container.transform.localScale = Vector3.one * 0.2f;
                blueprintCabinet.Prefab.AddComponent<PreferredUpgradeView>().Container = container.transform;
            }
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}

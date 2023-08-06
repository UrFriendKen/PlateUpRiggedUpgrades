using Kitchen;
using KitchenData;
using KitchenMods;
using UnityEngine;

namespace KitchenRiggedUpgrades
{
    public class PatchController : GenericSystemBase, IModSystem
    {
        static PatchController _instance;

        protected override void Initialise()
        {
            base.Initialise();
            _instance = this;
        }

        protected override void OnUpdate()
        {
        }

        internal static bool TryUpgrade(CDeskTarget target, float discount)
        {
            if (_instance == null ||
                !_instance.Require(target.Target, out CBlueprintStore blueprintStore) ||
                !blueprintStore.InUse ||
                blueprintStore.HasBeenUpgraded ||
                !GameData.Main.TryGet(blueprintStore.ApplianceID, out Appliance baseAppliance) ||
                !baseAppliance.HasUpgrades ||
                !(_instance?.Require(target.Target, out CPreferredUpgrade preferredUpgrade) ?? false) ||
                !GameData.Main.TryGet(preferredUpgrade.ApplianceID, out Appliance upgradedAppliance))
                return false;

            blueprintStore.Price = Mathf.CeilToInt((float)upgradedAppliance.PurchaseCost * discount);
            if (blueprintStore.HasBeenMadeFree)
            {
                blueprintStore.Price = Mathf.CeilToInt((float)blueprintStore.Price / 2f);
            }
            blueprintStore.ApplianceID = upgradedAppliance.ID;
            blueprintStore.BlueprintID = AssetReference.Blueprint;
            blueprintStore.HasBeenUpgraded = true;
            _instance.Set(target.Target, blueprintStore);
            _instance.EntityManager.RemoveComponent<CPreferredUpgrade>(target.Target);
            return true;
        }
    }
}

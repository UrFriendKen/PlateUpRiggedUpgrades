using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;

namespace KitchenRiggedUpgrades
{
    public class ManageUpgradeIndicators : PlayerSpecificUIIndicator<CUpgradeSelector, CUpgradeInfo>, IModSystem
    {
        protected override ViewType ViewType => Main.UpgradeSelectorViewType;

        protected override CUpgradeInfo GetInfo(Entity source, CUpgradeSelector selector, CTriggerPlayerSpecificUI trigger, CPlayer player)
        {
            CUpgradeInfo result = default;
            result.BlueprintStore = source;

            if (Require(source, out CBlueprintStore blueprintStore) &&
                GameData.Main.Has<Appliance>(blueprintStore.ApplianceID))
                result.ApplianceID = blueprintStore.ApplianceID;
            result.Player = player;
            result.PlayerEntity = trigger.TriggerEntity;
            return result;
        }

        protected override bool ShouldDismiss(CUpgradeInfo info)
        {
            if (info.ApplianceID == 0)
                return true;
            if (info.ClearPreferredUpgrade)
            {
                if (Has<CPreferredUpgrade>(info.BlueprintStore))
                    EntityManager.RemoveComponent<CPreferredUpgrade>(info.BlueprintStore);
            }
            else if (info.PreferredUpgradeID != 0 &&
                GameData.Main.TryGet(info.PreferredUpgradeID, out Appliance _, warn_if_fail: true) &&
                Require(info.BlueprintStore, out CBlueprintStore blueprintStore))
            {
                Set(info.BlueprintStore, new CPreferredUpgrade()
                {
                    ApplianceID = info.PreferredUpgradeID
                });
            }
            return base.ShouldDismiss(info);
        }
    }
}

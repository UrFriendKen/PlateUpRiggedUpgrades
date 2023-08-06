using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenRiggedUpgrades
{
    public class ManageUpgradeIndicators : IndicatorManager, IModSystem
    {
        protected override ViewType ViewType => Main.UpgradeSelectorViewType;

        protected override EntityQuery GetSearchQuery()
        {
            return GetEntityQuery(typeof(CBlueprintStore), typeof(CPosition));
        }

        protected override bool ShouldHaveIndicator(Entity candidate)
        {
            if (Require(candidate, out CHasIndicator comp))
            {
                if (!Require(comp.Indicator, out CUpgradeInfo upgradeInfo))
                {
                    return false;
                }
                if (!Has<CAppliance>(upgradeInfo.BlueprintStore) || !Require(upgradeInfo.BlueprintStore, out CBlueprintStore blueprintStore) || !blueprintStore.InUse)
                {
                    return false;
                }
                return !upgradeInfo.IsComplete;
            }
            if (!Has<CPosition>(candidate))
            {
                return false;
            }
            if (!Require(candidate, out CTriggerUpgradeSelector trigger))
            {
                return false;
            }
            if (!Has<CPlayer>(trigger.TriggerEntity))
            {
                return false;
            }
            if (!trigger.IsTriggered)
            {
                return false;
            }
            trigger.IsTriggered = false;
            Set(candidate, trigger);
            return true;
        }

        protected override Entity CreateIndicator(Entity source)
        {
            if (!Require(source, out CPosition position))
            {
                return default(Entity);
            }
            if (!Require(source, out CBlueprintStore blueprintStore))
            {
                return default(Entity);
            }
            if (!Require(source, out CTriggerUpgradeSelector trigger))
            {
                return default(Entity);
            }
            if (!Require<CPlayer>(trigger.TriggerEntity, out CPlayer player))
            {
                return default(Entity);
            }
            Entity entity = base.CreateIndicator(source);
            base.EntityManager.AddComponentData(entity, new CPosition(position));
            base.EntityManager.AddComponentData(entity, new CUpgradeInfo
            {
                ApplianceID = blueprintStore.ApplianceID,
                BlueprintStore = source,
                Player = player
            });
            return entity;
        }
    }
}

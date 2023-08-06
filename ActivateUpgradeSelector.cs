using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;

namespace KitchenRiggedUpgrades
{
    [UpdateBefore(typeof(ShowPingedCabinetInfo))]
    public class ActivateUpgradeSelector : ApplianceInteractionSystem, IModSystem
    {
        protected override InteractionType RequiredType => InteractionType.Notify;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require(data.Target, out CBlueprintStore blueprintStore) || !blueprintStore.InUse)
            {
                return false;
            }
            if (!GameData.Main.TryGet(blueprintStore.ApplianceID, out Appliance appliance) || !appliance.HasUpgrades)
            {
                return false;
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Set(data.Target, new CTriggerUpgradeSelector()
            {
                IsTriggered = true,
                TriggerEntity = data.Interactor
            });
        }
    }
}

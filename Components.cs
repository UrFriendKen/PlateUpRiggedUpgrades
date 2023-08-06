using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenRiggedUpgrades
{
    public struct CTriggerUpgradeSelector : IComponentData, IModComponent
    {
        public bool IsTriggered;
        public Entity TriggerEntity;
    }

    public struct CUpgradeInfo : IComponentData, IModComponent
    {
        public Entity BlueprintStore;
        public int ApplianceID;
        public InputIdentifier Player;
        public bool IsComplete;
    }

    public struct CPreferredUpgrade : IComponentData, IModComponent
    {
        public int ApplianceID;
    }
}

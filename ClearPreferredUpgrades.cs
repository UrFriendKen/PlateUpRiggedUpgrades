using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenRiggedUpgrades
{
    public class ClearPreferredUpgrades : GenericSystemBase, IModSystem
    {
        EntityQuery Cabinets;

        protected override void Initialise()
        {
            base.Initialise();
            Cabinets = GetEntityQuery(typeof(CBlueprintStore), typeof(CPreferredUpgrade));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = Cabinets.ToEntityArray(Allocator.Temp);
            using NativeArray<CBlueprintStore> blueprintStores = Cabinets.ToComponentDataArray<CBlueprintStore>(Allocator.Temp);
            using NativeArray<CPreferredUpgrade> preferredUpgrades = Cabinets.ToComponentDataArray<CPreferredUpgrade>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CBlueprintStore blueprintStore = blueprintStores[i];
                CPreferredUpgrade preferredUpgrade = preferredUpgrades[i];

                if (!blueprintStore.InUse ||
                    !GameData.Main.TryGet(blueprintStore.ApplianceID, out Appliance appliance) ||
                    !GameData.Main.TryGet(preferredUpgrade.ApplianceID, out Appliance preferredAppliance) ||
                    !appliance.Upgrades.Contains(preferredAppliance))
                {
                    EntityManager.RemoveComponent<CPreferredUpgrade>(entity);
                }
            }
        }
    }
}

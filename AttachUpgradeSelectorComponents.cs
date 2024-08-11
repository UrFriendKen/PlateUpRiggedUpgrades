using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenRiggedUpgrades
{
    public class AttachUpgradeSelectorComponents : RestaurantSystem, IModSystem
    {
        EntityQuery CabinetsWithoutUpgradeSelector;
        EntityQuery UpgradeSelectorsWithoutCustomTrigger;
        EntityQuery UpgradeSelectorsWithoutRequiresUpgrader;

        protected override void Initialise()
        {
            base.Initialise();
            CabinetsWithoutUpgradeSelector = GetEntityQuery(new QueryHelper()
                .All(typeof(CAppliance), typeof(CBlueprintStore), typeof(CCabinetModifier))
                .None(typeof(CUpgradeSelector)));

            UpgradeSelectorsWithoutCustomTrigger = GetEntityQuery(new QueryHelper()
                .All(typeof(CUpgradeSelector))
                .None(typeof(CCustomTriggerPlayerSpecificUI)));

            UpgradeSelectorsWithoutRequiresUpgrader = GetEntityQuery(new QueryHelper()
                .All(typeof(CUpgradeSelector))
                .None(typeof(CPlayerSpecificUIRequiresUpgrader)));
        }

        protected override void OnUpdate()
        {
            if (!CabinetsWithoutUpgradeSelector.IsEmpty)
            {
                using NativeArray<Entity> entities = CabinetsWithoutUpgradeSelector.ToEntityArray(Allocator.Temp);
                using NativeArray<CAppliance> appliances = CabinetsWithoutUpgradeSelector.ToComponentDataArray<CAppliance>(Allocator.Temp);

                for (int i = 0; i < entities.Length; i++)
                {
                    if (appliances[i].ID == -571205127) // Blueprint Cabinet
                    {
                        Entity entity = entities[i];

                        Set(entity, default(CUpgradeSelector));
                        Set(entity, default(CTriggerPlayerSpecificUI));
                        Set(entity, new CCustomTriggerPlayerSpecificUI()
                        {
                            Type = InteractionType.Notify,
                            OnlyInMode = true,
                            Mode = InteractionMode.Appliances
                        });
                    }
                }
            }

            if (!UpgradeSelectorsWithoutCustomTrigger.IsEmpty)
            {
                using NativeArray<Entity> entities = UpgradeSelectorsWithoutCustomTrigger.ToEntityArray(Allocator.Temp);

                for (int i = 0; i < entities.Length; i++)
                {
                    Set(entities[i], new CCustomTriggerPlayerSpecificUI()
                    {
                        Type = InteractionType.Notify,
                        OnlyInMode = true,
                        Mode = InteractionMode.Appliances
                    });
                }
            }

            if (!UpgradeSelectorsWithoutRequiresUpgrader.IsEmpty)
            {
                EntityManager.AddComponent<CPlayerSpecificUIRequiresUpgrader>(UpgradeSelectorsWithoutRequiresUpgrader);
            }
        }
    }
}

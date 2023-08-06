using Kitchen;
using KitchenData;
using KitchenMods;
using MessagePack;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenRiggedUpgrades
{
    public class PreferredUpgradeView : UpdatableObjectView<PreferredUpgradeView.ViewData>
    {
        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            EntityQuery Views;

            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CAppliance), typeof(CLinkedView), typeof(CBlueprintStore), typeof(CCabinetModifier));
            }

            protected override void OnUpdate()
            {
                using NativeArray<Entity> entities = Views.ToEntityArray(Allocator.Temp);
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using NativeArray<CCabinetModifier> cabinetModifiers = Views.ToComponentDataArray<CCabinetModifier>(Allocator.Temp);

                for (int i = 0; i < entities.Length; i++)
                {
                    Entity entity = entities[i];
                    CLinkedView view = views[i];
                    CCabinetModifier cabinetModifier = cabinetModifiers[i];

                    int applianceID = (cabinetModifier.Upgrades && Require(entity, out CPreferredUpgrade preferredUpgrade)) ? preferredUpgrade.ApplianceID : 0;
                    SendUpdate(views[i], new ViewData()
                    {
                        ApplianceID = applianceID
                    });
                }

            }
        }

        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int ApplianceID;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<PreferredUpgradeView>();

            public bool IsChangedFrom(ViewData check)
            {
                return ApplianceID != check.ApplianceID;
            }
        }

        public Transform Container;

        private int ApplianceID;

        protected override void UpdateData(ViewData data)
        {
            ApplianceID = data.ApplianceID;
            if (Container == null)
                return;
            Container.RemoveChildren();
            if (data.ApplianceID != 0 && GameData.Main.TryGet(data.ApplianceID, out Appliance appliance) && appliance.Prefab != null)
            {
                GameObject appliancePrefab = GameObject.Instantiate(appliance.Prefab);
                appliancePrefab.transform.SetParent(Container, false);
                appliancePrefab.transform.Reset();
            }
        }
    }
}

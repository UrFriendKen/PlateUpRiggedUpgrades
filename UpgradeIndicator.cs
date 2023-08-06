using Controllers;
using Kitchen;
using Kitchen.Modules;
using KitchenData;
using KitchenMods;
using KitchenRiggedUpgrades.Grids;
using MessagePack;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static Controllers.InputLock;

namespace KitchenRiggedUpgrades
{
    public class UpgradeIndicator : ResponsiveObjectView<UpgradeIndicator.ViewData, UpgradeIndicator.ResponseData>, IInputConsumer
    {
        public class UpdateView : ResponsiveViewSystemBase<ViewData, ResponseData>, IModSystem
        {
            EntityQuery Views;
            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CUpgradeInfo), typeof(CLinkedView));
            }

            protected override void OnUpdate()
            {
                using NativeArray<Entity> entities = Views.ToEntityArray(Allocator.Temp);
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using NativeArray<CUpgradeInfo> infos = Views.ToComponentDataArray<CUpgradeInfo>(Allocator.Temp);

                for (int i = 0; i < views.Length; i++)
                {
                    Entity entity = entities[i];
                    CLinkedView view = views[i];
                    CUpgradeInfo info = infos[i];

                    if (info.BlueprintStore == default)
                        continue;

                    SendUpdate(view.Identifier, new ViewData
                    {
                        Player = info.Player.PlayerID,
                        ApplianceID = info.ApplianceID
                    });

                    ResponseData result = default(ResponseData);
                    if (ApplyUpdates(view.Identifier,
                        delegate (ResponseData response)
                        {
                            result = response;
                        }, only_final_update: true))
                    {
                        info.IsComplete = result.IsComplete;
                        Set(entity, info);
                        if (result.ClearPreferredUpgrade)
                        {
                            if (Has<CPreferredUpgrade>(info.BlueprintStore))
                            {
                                EntityManager.RemoveComponent<CPreferredUpgrade>(info.BlueprintStore);
                            }
                        }
                        else if (result.UpgradedApplianceID != 0 &&
                            GameData.Main.TryGet(result.UpgradedApplianceID, out Appliance _, warn_if_fail: true) &&
                            Require(info.BlueprintStore, out CBlueprintStore blueprintStore))
                        {
                            blueprintStore.ApplianceID = result.UpgradedApplianceID;
                            Set(info.BlueprintStore, new CPreferredUpgrade()
                            {
                                ApplianceID = result.UpgradedApplianceID
                            });
                        }
                    }
                }
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : IViewData, IViewResponseData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)]
            public int Player;
            [Key(1)]
            public int ApplianceID;

            public bool IsChangedFrom(ViewData check)
            {
                return Player != check.Player || 
                    ApplianceID != check.ApplianceID;
            }
        }

        [MessagePackObject(false)]
        public struct ResponseData : IResponseData, IViewResponseData
        {
            [Key(0)]
            public bool IsComplete;
            [Key(1)]
            public bool ClearPreferredUpgrade;
            [Key(2)]
            public int UpgradedApplianceID;
        }

        private struct MenuStackElement
        {
            public GridMenuApplianceConfig Config;

            public int Index;
        }

        public GridMenuApplianceConfig RootMenuConfig;
        public Transform Container;
        private ApplianceGridMenu GridMenu;
        private int BaseApplianceID;
        private int UpgradedApplianceID;
        private bool ClearPreferredUpgrade;
        private int PlayerID;
        private InputLock.Lock Lock;
        private bool IsComplete;
        private Stack<MenuStackElement> MenuStack = new Stack<MenuStackElement>();

        private void CloseMenu()
        {
            if (MenuStack.Count > 1)
            {
                int index = MenuStack.Pop().Index;
                MenuStackElement menuStackElement = MenuStack.Pop();
                SetNewMenu(menuStackElement.Config, index, menuStackElement.Index);
            }
            else
            {
                Remove();
            }
        }

        private void SetNewMenu(GridMenuApplianceConfig menu, int new_index, int previous_index)
        {
            GridMenu?.Destroy();
            GridMenu = menu.Instantiate((GameData.Main.TryGet(BaseApplianceID, out Appliance appliance) ? appliance : null), delegate (int id)
            {
                UpgradedApplianceID = id;
                ClearPreferredUpgrade = id == 0;
                CloseMenu();
            }, Container, PlayerID, MenuStack.Count > 0);
            GridMenu.OnRequestMenu += delegate (GridMenuConfig c)
            {
                if (c is GridMenuApplianceConfig cApp)
                    SetNewMenu(cApp, 0, GridMenu?.SelectedIndex() ?? 0);
            };
            GridMenu.OnGoBack += CloseMenu;
            GridMenu.SelectByIndex(new_index);
            MenuStack.Push(new MenuStackElement
            {
                Config = menu,
                Index = previous_index
            });
        }

        protected override void UpdateData(ViewData data)
        {
            if (InputSourceIdentifier.DefaultInputSource != null)
            {
                if (!Players.Main.Get(data.Player).IsLocalUser)
                {
                    base.gameObject.SetActive(value: false);
                    return;
                }
                base.gameObject.SetActive(value: true);
                BaseApplianceID = data.ApplianceID;
                UpgradedApplianceID = 0;
                ClearPreferredUpgrade = false;
                InitialiseForPlayer(data.Player);
            }
        }

        private void InitialiseForPlayer(int player)
        {
            LocalInputSourceConsumers.Register(this);
            if (Lock.Type != 0)
            {
                InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
            }
            PlayerID = player;
            SetNewMenu(RootMenuConfig, 0, 0);
            Lock = InputSourceIdentifier.DefaultInputSource.SetInputLock(PlayerID, PlayerLockState.NonPause);
        }

        public override void Remove()
        {
            IsComplete = true;
            InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
            base.Remove();
        }

        private void OnDestroy()
        {
            LocalInputSourceConsumers.Remove(this);
        }

        public InputConsumerState TakeInput(int player_id, InputState state)
        {
            if (PlayerID != 0 && player_id == PlayerID)
            {
                if (state.MenuTrigger == ButtonState.Pressed)
                {
                    IsComplete = true;
                    InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
                    return InputConsumerState.Terminated;
                }
                if (GridMenu != null && !GridMenu.HandleInteraction(state) && state.MenuCancel == ButtonState.Pressed)
                {
                    CloseMenu();
                }
                if (!IsComplete)
                {
                    return InputConsumerState.Consumed;
                }
                return InputConsumerState.Terminated;
            }
            return InputConsumerState.NotConsumed;
        }

        public override bool HasStateUpdate(out IResponseData state)
        {
            state = null;
            if (IsComplete)
            {
                state = new ResponseData
                {
                    IsComplete = IsComplete,
                    ClearPreferredUpgrade = ClearPreferredUpgrade,
                    UpgradedApplianceID = UpgradedApplianceID
                };
            }
            return IsComplete;
        }
    }
}

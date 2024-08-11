using Kitchen;
using KitchenData;
using KitchenMods;
using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using UnityEngine;

namespace KitchenRiggedUpgrades
{
    [Obsolete("Now uses CUpgradeSelector")]
    public struct CTriggerUpgradeSelector : IComponentData, IModComponent
    {
        public bool IsTriggered;
        public Entity TriggerEntity;
    }

    public struct CCustomTriggerPlayerSpecificUI : IApplianceProperty, IComponentData, IModComponent
    {
        public InteractionType Type;

        public InteractionMode Mode;

        public bool OnlyInMode;
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CPlayerSpecificUIRequiresUpgrader : IApplianceProperty, IComponentData, IModComponent
    {
    }

    public struct CUpgradeSelector : IComponentData, IPlayerSpecificUISource, IModComponent
    {
        public Vector3 DrawLocation;

        Vector3 IPlayerSpecificUISource.DrawLocation => DrawLocation;
    }

    public struct CUpgradeInfo : IComponentData, IPlayerSpecificUI, IModComponent
    {
        public Entity PlayerEntity;
        public InputIdentifier Player;
        public bool IsComplete;

        public Entity BlueprintStore;
        public int ApplianceID;

        public int PreferredUpgradeID;
        public bool ClearPreferredUpgrade;

        Entity IPlayerSpecificUI.PlayerEntity => PlayerEntity;

        bool IPlayerSpecificUI.IsComplete => IsComplete;
    }

    public struct CPreferredUpgrade : IComponentData, IModComponent
    {
        public int ApplianceID;
    }
}

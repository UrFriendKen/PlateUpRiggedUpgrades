using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenRiggedUpgrades
{
    [UpdateInGroup(typeof(HighPriorityInteractionGroup))]
    public class ActivateCustomPlayerSpecificUI : InteractionSystem, IModSystem
    {
        protected override bool AllowAnyMode => true;
        protected override bool AllowActOrGrab => true;
        protected override InteractionType RequiredType => InteractionType.Notify;

        private CCustomTriggerPlayerSpecificUI CustomTrigger;

        private CTriggerPlayerSpecificUI Editor;

        private COwnedByPlayer OwnedByPlayer;

        protected override bool ShouldAct(ref InteractionData data)
        {
            if (!Require(data.Target, out Editor))
                return false;

            if (!Require(data.Target, out CustomTrigger))
                return false;

            if (CustomTrigger.Type != data.Attempt.Type ||
                CustomTrigger.OnlyInMode && CustomTrigger.Mode != data.Attempt.Mode)
                return false;

            bool res = base.ShouldAct(ref data);
            Main.LogInfo(res);
            return res;
        }

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Has<CCustomTriggerPlayerSpecificUI>(data.Target))
            {
                return false;
            }
            if (!Require(data.Target, out Editor))
            {
                return false;
            }
            if (Require(data.Target, out OwnedByPlayer) && OwnedByPlayer.Player != data.Interactor)
            {
                return false;
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Editor.IsTriggered = true;
            Editor.TriggerEntity = data.Interactor;
            SetComponent(data.Target, Editor);
        }
    }
}

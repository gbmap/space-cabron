using Frictionless;
using Gmap.ScriptableReferences;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay.Combo
{
    public class ComboSystem : MonoBehaviour
    {
        public int PlayerIndex = 0;
        private int CurrentCombo = 0;

        public FloatBusReference ComboTime;

        void Update() {
            if (ComboTime.Value <= 0f) {
                return;
            }

            ComboTime.Value -= Time.deltaTime/3;
            if (ComboTime.Value <= 0f) {
                MessageRouter.RaiseMessage(new Messages.MsgOnComboBroken{
                    PlayerIndex = PlayerIndex,
                    Combo = CurrentCombo
                });
                CurrentCombo = 0;
            }
        }

        void OnEnable() {
            MessageRouter.AddHandler<Messages.MsgOnNotePlayedInTime>(Callback_OnNotePlayedInTime);
            MessageRouter.AddHandler<Messages.MsgOnNotePlayedOutOfTime>(Callback_OnNotePlayedOutOfTime);
            MessageRouter.AddHandler<Gmap.Gameplay.MsgOnObjectHit>(Callback_OnObjectHit);
            MessageRouter.AddHandler<Messages.MsgOnComboIncrease>(Callback_OnComboIncreased);
        }


        void OnDisable() {
            MessageRouter.RemoveHandler<Messages.MsgOnNotePlayedInTime>(Callback_OnNotePlayedInTime);
            MessageRouter.RemoveHandler<Messages.MsgOnNotePlayedOutOfTime>(Callback_OnNotePlayedOutOfTime);
            MessageRouter.RemoveHandler<Gmap.Gameplay.MsgOnObjectHit>(Callback_OnObjectHit);
            MessageRouter.RemoveHandler<Messages.MsgOnComboIncrease>(Callback_OnComboIncreased);
        } 
        
        private void Callback_OnNotePlayedInTime(MsgOnNotePlayedInTime msg)
        {      
            if (PlayerIndex != msg.PlayerIndex)
                return;

            MessageRouter.RaiseMessage(new Messages.MsgOnComboIncrease {
                PlayerIndex = this.PlayerIndex,
                CurrentCombo = ++CurrentCombo
            });
        }

        private void Callback_OnWrongBulletHit(Messages.MsgOnWrongBulletHit obj) {
            if (obj.PlayerIndex != PlayerIndex)
                return;

            MessageRouter.RaiseMessage(new Messages.MsgOnComboBroken{
                Combo = CurrentCombo,
                PlayerIndex = PlayerIndex
            });
            CurrentCombo = 0;
        }

        private void Callback_OnNotePlayedOutOfTime(Messages.MsgOnNotePlayedOutOfTime obj) {
            if (obj.PlayerIndex != PlayerIndex)
                return;

            MessageRouter.RaiseMessage(new Messages.MsgOnComboBroken {
                PlayerIndex = PlayerIndex,
                Combo = CurrentCombo
            });
            CurrentCombo = 0;
        }

        private void Callback_OnObjectHit(Gmap.Gameplay.MsgOnObjectHit msg) {
            if (msg.objectFiring as PlayerFire == null) {
                return;
            }

            var playerFire = msg.objectFiring as PlayerFire;
            if (playerFire.Brain as ScriptableInputBrain == null) {
                return;
            }

            var brain = playerFire.Brain as ScriptableInputBrain;

            if (brain.Index != PlayerIndex)
                return;

            MessageRouter.RaiseMessage(new Messages.MsgOnComboIncrease {
                PlayerIndex = this.PlayerIndex,
                CurrentCombo = ++CurrentCombo
            });
        }

        private void Callback_OnComboIncreased(MsgOnComboIncrease obj)
        {
            ComboTime.Value = 1f;
        }
    }
}
using System;
using Frictionless;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay.Combo
{
    public class ComboSystem : MonoBehaviour
    {
        public int PlayerIndex = 0;
        private int CurrentCombo = 0;

        void OnEnable() {
            MessageRouter.AddHandler<Messages.MsgOnNotePlayedInTime>(Callback_OnNotePlayedInTime);
            MessageRouter.AddHandler<Messages.MsgOnNotePlayedOutOfTime>(Callback_OnNotePlayedOutOfTime);
            MessageRouter.AddHandler<Messages.MsgOnWrongBulletHit>(Callback_OnWrongBulletHit);
            MessageRouter.AddHandler<Gmap.Gameplay.MsgOnObjectHit>(Callback_OnObjectHit);
        }

       

        void OnDisable() {
            MessageRouter.RemoveHandler<Messages.MsgOnNotePlayedInTime>(Callback_OnNotePlayedInTime);
            MessageRouter.RemoveHandler<Messages.MsgOnNotePlayedOutOfTime>(Callback_OnNotePlayedOutOfTime);
            MessageRouter.RemoveHandler<Messages.MsgOnWrongBulletHit>(Callback_OnWrongBulletHit);
            MessageRouter.RemoveHandler<Gmap.Gameplay.MsgOnObjectHit>(Callback_OnObjectHit);
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
    }
}
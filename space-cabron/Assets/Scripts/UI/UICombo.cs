using System;
using Frictionless;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.UI {
    public class UICombo : MonoBehaviour
    {
        public int PlayerIndex;
        private TMPro.TextMeshProUGUI text;
        private Animator animator;
        
        void Awake() {
            text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            animator = GetComponentInChildren<Animator>();
        }

        void OnEnable()
        {
            MessageRouter.AddHandler<Messages.MsgOnComboIncrease>(Callback_OnComboIncrease);
            MessageRouter.AddHandler<Messages.MsgOnComboBroken>(Callback_OnComboBroken);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.MsgOnComboIncrease>(Callback_OnComboIncrease);
            MessageRouter.RemoveHandler<Messages.MsgOnComboBroken>(Callback_OnComboBroken);
        }

        private void Callback_OnComboIncrease(Messages.MsgOnComboIncrease msg)
        {
            if (PlayerIndex != msg.PlayerIndex)
                return;
                
            if (text) {
                text.text = msg.CurrentCombo.ToString();
                text.enabled = true;
            }

            if (animator) {
                animator.SetTrigger("Shake");
            }
        }

        private void Callback_OnComboBroken(MsgOnComboBroken obj)
        {
            if (obj.PlayerIndex != PlayerIndex)
                return;
            
            if (text) {
                text.text = "0";
                text.enabled = false;
            }
        }
    }
}
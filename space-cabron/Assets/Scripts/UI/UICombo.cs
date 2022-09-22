using System;
using Frictionless;
using Gmap.ScriptableReferences;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.UI {
    public class UICombo : MonoBehaviour
    {
        public int PlayerIndex;
        public FloatBusReference ComboTime;
        private TMPro.TextMeshProUGUI text;
        private Animator animator;

        private UnityEngine.UI.RawImage imageComboTime;
        
        void Awake() {
            text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            animator = GetComponentInChildren<Animator>();
            imageComboTime = GetComponentInChildren<UnityEngine.UI.RawImage>();
        }

        void Update() {
            if (imageComboTime) {
                imageComboTime.transform.localScale = new Vector3(ComboTime.Value, 1, 1);
            }
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
                imageComboTime.enabled = true;
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
                imageComboTime.enabled = false;
            }
        }
    }
}
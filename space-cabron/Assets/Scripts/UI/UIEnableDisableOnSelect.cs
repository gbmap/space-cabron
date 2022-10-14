using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceCabron.UI
{
    public class UIEnableDisableOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Behaviour behaviour;
        public AudioClip SoundEffectOverride;

        void OnDisable() {
            behaviour.enabled = false;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            behaviour.enabled = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            AudioClip clip = Resources.Load<AudioClip>("Sounds/UI/UI_blipSelect");
            if (SoundEffectOverride) {
                clip = SoundEffectOverride;
            }

            behaviour.enabled = true;
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }
    }
}
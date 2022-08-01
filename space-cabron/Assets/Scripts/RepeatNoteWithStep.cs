using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class RepeatNoteWithStep : MonoBehaviour
    {
        public int Steps = 3;
        public float Probability = 0.1f;
        public TurntableBehaviour Turntable;
        public HelmProxy Proxy;

        public System.Action<OnNoteArgs> OnNoteRepeated;

        void Start()
        {
            Turntable.UnityEvent.AddListener(OnNote);
        }

        void OnDestroy()
        {
            Turntable.UnityEvent.RemoveListener(OnNote);
        }

        void OnNote(OnNoteArgs args)
        {
            if (Random.value < Probability)
            {
                OnNoteArgs args2 = new OnNoteArgs
                {
                    Duration = args.Duration,
                    HoldTime = args.HoldTime,
                    Note = Note.Transpose(args.Note, Steps)
                };

                Proxy.Play(args2);
                OnNoteRepeated?.Invoke(args2);
            }
        }
    }
}
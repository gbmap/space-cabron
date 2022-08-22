using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Gmap.CosmicMusicUtensil
{
    public class RepeatNoteWithStep : MonoBehaviour
    {
        public int Steps = 3;
        public float Probability = 0.1f;
        public ScriptableSelectionStrategy SelectionStrategy;
        public TurntableBehaviour Turntable;
        public HelmProxy Proxy;

        public System.Action<OnNoteArgs> OnNoteRepeated;
        public UnityEvent<OnNoteArgs> UnityEvent;

        public float LastNote { get; private set; }

        public void SetTurntable(TurntableBehaviour turntable)
        {
            Turntable = turntable;
            Turntable.UnityEvent.AddListener(OnNote);
        }

        void Start()
        {
            if (Turntable == null)
            {
                Destroy(this);
                return;
            }

            SetTurntable(Turntable);
        }

        void OnDestroy()
        {
            if (Turntable == null)
                return;

            Turntable.UnityEvent.RemoveListener(OnNote);
        }

        void OnNote(OnNoteArgs args)
        {
            if (this.SelectionStrategy.Get().ShouldSelect(Turntable.Melody.NoteArray, Turntable.NoteIndex))
            {
                OnNoteArgs args2 = new OnNoteArgs
                {
                    Duration = args.Duration,
                    HoldTime = args.HoldTime,
                    Note = Note.TransposeWrapped(args.Note, Steps)
                };

                Proxy.Play(args2);
                OnNoteRepeated?.Invoke(args2);
                UnityEvent?.Invoke(args2);

                LastNote = Time.time;
            }
        }

        public void UpdateReferences(GameObject target)
        {
            Turntable = target.GetComponentInChildren<TurntableBehaviour>();
            Turntable.OnNote += OnNote;

            Proxy = target.GetComponentInChildren<HelmProxy>();
        }
    }
}
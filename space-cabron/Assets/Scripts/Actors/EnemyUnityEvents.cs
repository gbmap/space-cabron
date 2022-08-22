using System;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.Gameplay
{
    public class EnemyUnityEvents : MonoBehaviour
    {
        public Melody Melody;

        public UnityEvent<OnNoteArgs> OnEnemyHit;
        public UnityEvent OnEnemyDestroyed;

        private int noteIndex = 0;

        void Awake()
        {
            this.Melody = new Melody(Melody);
            MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnEnemyHit);
        }

        void OnDestroy()
        {
            MessageRouter.RemoveHandler<MsgOnObjectDestroyed>(Callback_OnEnemyHit);
        }

        private void Callback_OnEnemyHit(MsgOnObjectHit obj)
        {
            if (!obj.health.CompareTag("Enemy"))
                return;

            Note n = Melody.GetNote(noteIndex++);
            OnEnemyHit?.Invoke(new OnNoteArgs
            {
                Note = n,
                HoldTime = 0.7f
            });
        }
    }
}
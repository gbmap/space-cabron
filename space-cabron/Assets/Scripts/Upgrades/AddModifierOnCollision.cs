using UnityEngine;
using Gmap.CosmicMusicUtensil;
using Gmap.Utils;
using SpaceCabron.Gameplay;

namespace Gmap
{
    public abstract class CollisionHandler : MonoBehaviour
    {
        public bool HandleOnlyOnce = true;
        protected bool hasHandled;
        public void OnTriggerEnter2D(Collider2D other)
        {
            TurntableBehaviour t = TurntableResolver.Create("GlobalInstruments","PlayerInstrument").Get();
            if (t == null)
                return;

            if (!HandleOnlyOnce || (HandleOnlyOnce && !hasHandled))
                HandleCollision(t);
        }

        protected abstract void HandleCollision(TurntableBehaviour t);
    }

    public class AddModifierOnCollision : CollisionHandler
    {
        public MelodyModifier.EType ModifierType;

        private static ShuffleBag<int> BreakNoteShuffleBag;

        void Awake()
        {
            if (BreakNoteShuffleBag == null)
            {
                BreakNoteShuffleBag = new ShuffleBag<int>();
                BreakNoteShuffleBag.Add(2, 10);
                BreakNoteShuffleBag.Add(3, 3);
                BreakNoteShuffleBag.Add(4, 1);
            }

        }

        protected override void HandleCollision(TurntableBehaviour t)
        {
            MelodyModifier m = ModifierFromType(ModifierType);
            t.SetMelody(m.Apply(t.Melody));
        }

        private MelodyModifier ModifierFromType(MelodyModifier.EType t)
        {
            switch (t)
            {
                case MelodyModifier.EType.BreakNote:
                {
                    var modifier = new BreakMelodyNoteModifier(BreakNoteShuffleBag.Next());
                    return modifier;
                }
                case MelodyModifier.EType.ShiftNote:
                {
                    var modifier = new ShiftNoteMelodyModifier(Random.Range(-12,12));
                    return modifier;
                }
                case MelodyModifier.EType.TransposeMelody:
                {
                    return new TransposeMelodyModifier(Random.Range(0, 5), Random.Range(-12, 12));
                }
                default:
                    return null;
            }
        }
    }
}
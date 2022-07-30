using UnityEngine;
using Gmap.CosmicMusicUtensil;
using Gmap.Utils;

namespace Gmap
{
    public abstract class CollisionHandler<T> : MonoBehaviour where T : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            T t = other.GetComponentInChildren<T>();
            if (t == null)
                return;

            HandleCollision(t);
        }

        protected abstract void HandleCollision(T t);
    }

    public class AddModifierOnCollision : CollisionHandler<TurntableBehaviour>
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
            t.SetMelody(m.Apply(t.melody));
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
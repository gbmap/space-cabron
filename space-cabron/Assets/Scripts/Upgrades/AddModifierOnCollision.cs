using UnityEngine;
using Gmap.CosmicMusicUtensil;
using Utils;

namespace SpaceCabron
{
    public class AddModifierOnCollision : MonoBehaviour
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

        public void OnTriggerEnter2D(Collider2D other)
        {
            TurntableBehaviour t = other.GetComponentInChildren<TurntableBehaviour>();
            if (t == null)
                return;

            MelodyModifier m = ModifierFromType(ModifierType);
            t.SetMelody(m.Apply(t.Melody));
        }

        public MelodyModifier ModifierFromType(MelodyModifier.EType t)
        {
            switch (t)
            {
                case MelodyModifier.EType.BreakNote:
                {
                    var modifier = new BreakNoteModifier();
                    modifier.NumberOfNotes = BreakNoteShuffleBag.Next();
                    return modifier;
                }
                case MelodyModifier.EType.ShiftNote:
                {
                    var modifier = new ShiftNoteModifier();
                    modifier.Step = Random.Range(-12, 12);
                    return modifier;
                }
                default:
                    return null;
            }
        }
    }
}
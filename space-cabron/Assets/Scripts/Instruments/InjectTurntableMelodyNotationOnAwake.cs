using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap.Instruments
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class InjectTurntableMelodyNotationOnAwake : Injectable
    {
        public Gmap.ScriptableReferences.StringReferencePool PossibleMelodies;
        public ENote Root;
        public bool RandomizeRoot;
        public ScriptableScale Scale;

        void Start()
        {
            if (RandomizeRoot)
                Root = (ENote)Random.Range(0, 12);

            ITurntable behaviour = GetComponent<ITurntable>();
            Melody m = new Melody(PossibleMelodies.GetNext().Value);
            if (Scale)
            {
                Note n = m.GetNote(0);
                n.Tone = Root;
                for (int i = 1; i < m.Length; i++)
                {
                    n = m.GetNote(i);
                    n.Tone = Scale.GetNote(Root, Random.Range(0, 100));
                }
            }

            behaviour.SetMelody(m);
            Destroy(this);
        }
    }
}
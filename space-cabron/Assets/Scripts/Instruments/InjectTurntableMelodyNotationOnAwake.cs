using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron.Instruments
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class InjectTurntableMelodyNotationOnAwake : MonoBehaviour 
    {
        public Gmap.ScriptableReferences.StringReferencePool PossibleMelodies;
        TurntableBehaviour behaviour;

        void Awake()
        {
            behaviour = GetComponent<TurntableBehaviour>();
            behaviour.SetMelody(new Melody(PossibleMelodies.GetNext().Value));
            Destroy(this);
        }
    }
}
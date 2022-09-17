using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay {
    public class RandomizeInstrumentOnKeyPress : MonoBehaviour, IBrainHolder<InputState>
    {
        IBrain<InputState> brain;
        public IBrain<InputState> Brain { get => brain; set => brain = value; }

        void Update()
        {
            if (Brain == null)
                return;

            if (Brain.GetInputState(new InputStateArgs {
                Caller = this,
                Object = gameObject
            }).RandomizeInstrument)
                GetComponentInChildren<HelmProxy>().Randomize();
        }
    }
}
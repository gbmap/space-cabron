using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil {
    public class MelodySwitcher : MonoBehaviour, IMelodyPlayer
    {
        public TurntableBehaviour Turntable;
        public string Structure = "1212";

        public const int MAX_MELODIES = 8;
        private Melody[] melodies = new Melody[MAX_MELODIES];

        public StringReferencePool MelodyPatterns;
        SongStructureGenerator songStructureGenerator;

        void Awake() {
            Turntable.OnBar += Callback_OnBar;

            songStructureGenerator = new SongStructureGenerator(this, MelodyPatterns);
            songStructureGenerator.Generate();
        }

        void Start() {
            Turntable.SetMelody(melodies[0]);
        }

        private void Callback_OnBar(OnBarArgs msg)
        {
            int structureIndex= msg.BarIndex % Structure.Length;
            if (structureIndex == 0) {
                songStructureGenerator.Generate();
            }
            int melodyIndex = Structure[structureIndex]-'0'-1;
            if (melodies[melodyIndex] == null)
                return;
            Turntable.SetMelody(melodies[melodyIndex]);
        }

        public void GenerateMelodies(MelodyFactory factory) {
            for (int i = 0; i < melodies.Length; i++)
                melodies[i] = factory.GenerateMelody();
            Turntable.SetMelody(melodies[0]);
        }

        public void Generate(MelodyFactory factory)
        {
            GenerateMelodies(factory);
        }

        public int MelodyPlayerPriority => 2;
    }
}
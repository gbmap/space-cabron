using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.CosmicMusicUtensil
{
    public class TurntableBehaviour : MonoBehaviour
    {
        public int BPM = 60;
        public ScriptableCompositeBar Bar;
        public UnityEvent<OnNoteArgs> UnityEvent;

        ITurntable _turntable;

        void Awake()
        {
            var generator = new WaveBarGenerator(new WaveBarGeneratorConfig
            {
                BaseOctave = 3,
                Amplitude = 1
            });
            var bar = ScriptableConverter.ToScriptable(generator.Generate(new BarGeneratorParams
            {
                MaxNotes = 12,
                Scale = new Scale {
                    Intervals = new int[] { 2, 1, 2, 2, 1, 2, 2 }
                }
            }));
            _turntable = new Turntable(BPM, bar);
        }

        void Update()
        {
            _turntable.Update(OnNote);
        }

        void OnNote(OnNoteArgs note)
        {
            UnityEvent.Invoke(note);
        }
    }
}
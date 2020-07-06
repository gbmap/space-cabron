using System;
using UnityEngine;

namespace Z
{

    public class ZNoteAlternation
    {
        public int NoteIndex = 0;
        public int SemitoneOffset = 1;
    }

    public abstract class ZBaseMelodyPlayer : MonoBehaviour
    {
        [Header("References")]
        public ZBaseMarchPlayer marcher;

        [Header("Current Note")]
        public int currentNoteIndex;

        [Header("Silence")]
        public bool addSilence;

        [Range(0f, 1f)]
        public float silenceProbability = 0.25f;

        public ZMelodyImproviser Improviser { get; set; }

        private ZMelody _improvisedMelodyCache;
        private ZMelody _baseMelody = ZMelody.Empty;
        public ZMelody BaseMelody
        {
            get { return _baseMelody; }
            set
            {
                _baseMelody = value;
                if (Improviser != null && Improviser.HasImprovisers)
                {
                    _improvisedMelodyCache = Improviser.Improvise(value, value.CurrentNoteIndex);
                }
            }
        }

        public ZMelody Melody
        {
            get
            {
                if (Improviser != null && Improviser.HasImprovisers)
                {
                    return _improvisedMelodyCache;
                }
                return BaseMelody;
            }
            set
            {
                BaseMelody = value;
            }
        }

        public ENote CurrentNote
        {
            get
            {
                return Melody.CurrentNote;
            }
        }

        public int CurrentOctave
        {
            get
            {
                return Melody.CurrentOctave;
            }
        }

        private void Start()
        {
            Improviser = new ZMelodyImproviser();
        }

        private void OnEnable()
        {
            marcher.OnBeat += OnBeat;
            marcher.OnBar += OnBar;
            marcher.OnNewBeat += OnNewBeat;
        }

        private void OnDisable()
        {
            marcher.OnBeat -= OnBeat;
            marcher.OnBar -= OnBar;
            marcher.OnNewBeat -= OnNewBeat;
        }

        private void OnBeat(int beatType)
        {
            if (Melody.IsEmpty) return;

            Play(CurrentNote, CurrentOctave);
            Melody.Advance();
        }

        private void OnNewBeat(int obj)
        {
            //GenerateMelody();
        }

        private void OnBar(int obj)
        {
            _improvisedMelodyCache = Improviser.Improvise(BaseMelody, obj);
        }

        public abstract void Play(ENote note, int octave);
    }


    /*
     * Plays melodies with N notes (from the assigned Marcher)
     * through an assigned Synth.
     * */
    public class ZMelodyPlayerSynth : ZBaseMelodyPlayer
    {
        public Synth Synth;

        public override void Play(ENote note, int octave)
        {
            Synth.PlayKey(CurrentNote, CurrentOctave);
        }
    }
}
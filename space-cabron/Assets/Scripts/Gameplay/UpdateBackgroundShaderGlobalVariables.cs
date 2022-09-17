using System.Linq;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap
{
    public class UpdateBackgroundShaderGlobalVariables : MonoBehaviour
    {
        const int nNotes = 100;
        float[] LastNoteTimes = new float[nNotes];
        float[] NoteTimes = new float[nNotes];
        float[] NextNoteTimes = new float[nNotes];

        float expectedNextNote = 0f;

        int noteTimesCursor = 0;

        int Index
        {
            get { return Mathf.Clamp(gameObject.name[gameObject.name.Length - 1] - '0', 0, 1); }
        }

        void Update()
        {
            Shader.SetGlobalFloat("_EngineTime", Time.time);
        }

        public void OnNote(OnNoteArgs n)
        {
            float delta = Time.time - expectedNextNote;
            // System.Array.ForEach(LastNoteTimes, (x) => x += delta);
            System.Array.ForEach(NoteTimes, (x) => x += delta);
            // System.Array.ForEach(NextNoteTimes, (x) => x += delta);
            UpdateBuffers();

            Shader.SetGlobalFloat("_Beat", Time.time);
            Shader.SetGlobalFloat("_LastNoteDuration", n.Duration);

            expectedNextNote = Time.time + n.Duration;
        }

        public void OnBar(OnBarArgs args)
        {
            float startTime = Time.time + args.Turntable.LastNote.GetDuration(args.Turntable.BPS);

            MelodySwitcher ms = GetComponentInChildren<MelodySwitcher>();
            if (ms != null) {
                var improviser = args.Turntable.Improviser;

                int i = args.BarIndex % ms.Structure.Length;
                var melody = ms.GetMelody(ms.Structure[i]-'0'-1);
                startTime = UpdateNoteTimes(args, melody, improviser, startTime, args.Turntable.BarIndex, NoteTimes);

                int i2 = (args.BarIndex+1) % ms.Structure.Length;
                var melody2 = ms.GetMelody(ms.Structure[i2]-'0'-1);
                UpdateNoteTimes(args, melody2, improviser, startTime, args.Turntable.BarIndex+1, NoteTimes);
            }
            else {
                startTime = UpdateNoteTimes(args, args.Turntable.Melody, args.Turntable.Improviser, startTime, args.Turntable.BarIndex, NoteTimes);
                UpdateNoteTimes(args, args.Turntable.Melody, args.Turntable.Improviser, startTime, args.Turntable.BarIndex + 1, NoteTimes);
            }

            UpdateBuffers();
        }

        private void UpdateBuffers()
        {
            Shader.SetGlobalInteger("_NoteCount" + Index.ToString(), nNotes);
            // Shader.SetGlobalFloatArray("_LastNoteTimes" + Index.ToString(), LastNoteTimes);
            Shader.SetGlobalFloatArray("_NoteTimes" + Index.ToString(), NoteTimes);
            // Shader.SetGlobalFloatArray("_NextNoteTimes" + Index.ToString(), NextNoteTimes);
        }

        private void CacheLastNoteTimes()
        {
            System.Array.Copy(NoteTimes, 0, LastNoteTimes, 0, NoteTimes.Length);
        }

        private float UpdateNoteTimes(OnBarArgs args,
                                     Melody melody,
                                     Improviser improviser,
                                     float startTime,
                                     int barIndex,
                                     float[] noteTimes)
        {
            var turntable = args.Turntable;
            // var melody = args.Turntable.Melody;
            // var improviser = args.Turntable.Improviser;
            var notes = Enumerable.Range(0, melody.Length)
                      .SelectMany(i => improviser.Improvise(melody, barIndex, melody.GetNote(i), i, false))
                      .ToArray();

            float value = startTime;
            // noteTimes[noteTimesCursor] = value;
            for (int i = 0; i < notes.Length; i++)
            {
                value += notes[i].GetDuration(args.Turntable.BPS);
                noteTimes[noteTimesCursor] = value;
                noteTimesCursor = (noteTimesCursor + 1) % noteTimes.Length;
            }
            return value;
        }
    }
}
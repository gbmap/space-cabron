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

        void Update()
        {
            Shader.SetGlobalFloat("_EngineTime", Time.time);
        }

        public void OnNote(OnNoteArgs n)
        {
            Shader.SetGlobalFloat("_Beat", Time.time);
            Shader.SetGlobalFloat("_LastNoteDuration", n.Duration);
        }

        public void OnBar(OnBarArgs args)
        {
            System.Array.ForEach(NoteTimes, x => x = 0);
            System.Array.ForEach(NextNoteTimes, x => x = 0);

            CacheLastNoteTimes();

            float startTime = Time.time + args.Turntable.LastNote.GetDuration(args.Turntable.BPS);
            startTime = UpdateNoteTimes(args, startTime, args.Turntable.BarIndex, NoteTimes);
            UpdateNoteTimes(args, startTime, args.Turntable.BarIndex + 1, NextNoteTimes);

            Shader.SetGlobalInteger("_NoteCount", nNotes);
            Shader.SetGlobalFloatArray("_LastNoteTimes", LastNoteTimes);
            Shader.SetGlobalFloatArray("_NoteTimes", NoteTimes);
            Shader.SetGlobalFloatArray("_NextNoteTimes", NextNoteTimes);
        }

        private void CacheLastNoteTimes()
        {
            System.Array.Copy(NoteTimes, 0, LastNoteTimes, 0, NoteTimes.Length);
        }

        private float UpdateNoteTimes(OnBarArgs args,
                                     float startTime,
                                     int barIndex,
                                     float[] noteTimes)
        {
            var turntable = args.Turntable;
            var melody = args.Turntable.Melody;
            var improviser = args.Turntable.Improviser;
            var notes = Enumerable.Range(0, melody.Length)
                      .SelectMany(i => improviser.Improvise(melody, barIndex, melody.GetNote(i), i, false))
                      .ToArray();

            float value = startTime;
            noteTimes[0] = value;
            for (int i = 0; i < notes.Length; i++)
            {
                value += notes[i].GetDuration(args.Turntable.BPS);
                noteTimes[i+1] = value;
            }
            return value;
        }
    }
}
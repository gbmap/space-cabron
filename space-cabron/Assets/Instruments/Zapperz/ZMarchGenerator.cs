using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Z
{
    public enum EMarchGenerationAlgorithm
    {
        Fixed,
        Procrastinator
    }

    /*
     * 
     * BEAT GENERATOR
     * Algorithms to generate beats.
     * 
     * */
    [CommandPrefix("marchGen.")]
    public static class ZMarchGenerator
    {
        ///////////////////
        /// UTILITY
        /// 
        private static Dictionary<int, float> TimeFractions = new Dictionary<int, float>
    {
        { 1, 4f },      // whole note = beattime / 0.25 or *4
        { 2, 2f },      // half note = beattime / 0.5 or *2
        { 4, 1f },      // 4th note = beattime  / 1    or *1
        { 8, 0.5f },    // 8th note = beattime / 2  or * 0.5
        { 16, 0.25f },  // 16th note = beattime / 4  or * 0.25
        { 32, 0.125f }, // 32th note = beattime / 8  or * 0.125
        { 64, 0.0625f }
    };

        private static Dictionary<int, List<int[]>> _possibleBeats;
        private static Dictionary<int, List<int[]>> PossibleBeats
        {
            get
            {
                if (_possibleBeats == null) LoadPossibleBeats();
                return _possibleBeats;
            }
        }


        [Command("loadBeats")]
        private static void LoadPossibleBeats()
        {
            _possibleBeats = new Dictionary<int, List<int[]>>();

            TextAsset[] beatFiles = Resources.LoadAll<TextAsset>("BeatData/");
            foreach (var beatFile in beatFiles)
            {
                if (string.IsNullOrEmpty(beatFile.text)) continue;

                List<int[]> beats = new List<int[]>();

                var beatLines = beatFile.text.Replace("\r", "").Split('\n');
                foreach (var beatLine in beatLines)
                {
                    if (string.IsNullOrEmpty(beatLine)) continue;
                    var notesStr = beatLine.Split(',');
                    var notes = notesStr.Select(bl => Convert.ToInt32(bl)).ToArray();
                    beats.Add(notes);
                }

                _possibleBeats[Convert.ToInt32(beatFile.name)] = beats;
            }
        }
        
        private static float GetTimeFraction(int noteType)
        {
            return 4f / noteType;
        }

        public static float BPMtoBPS(int BPM)
        {
            return (float)BPM / 60;
        }

        // quarter beat time
        public static float GetQuarterBeatTime(int BPM)
        {
            return (float)60 / BPM;
        }

        public static float GetBeatType(int BPM, int noteType)
        {
            float timeFraction = GetTimeFraction(noteType);
            return GetQuarterBeatTime(BPM) * timeFraction;
        }

        public static float GetBarTime(int BPM, int notesInBar, int noteType)
        {
            float bps = BPMtoBPS(BPM);
            float qbt = GetQuarterBeatTime(BPM); // quarter beat time

            float nt = GetTimeFraction(noteType);
            return (qbt * nt) * notesInBar;
        }

        public static int BreakNote(int noteType)
        {
            return Mathf.Min(32, noteType << 1);
        }

        /*
         * Generates random note times in a space of N notes,
         * that fit a specified bar time calculated through
         * BPM and a time signature.
         * 
         * */
        [Command("fixed")]
        public static ZMarch GenerateFixedBeat(int nNotes)
        {
            int groupsSz = Mathf.Max(1, nNotes - 2);

            //int[] group = GetGroup(nNotes, UnityEngine.Random.Range(0, groupsSz));
            int[] group = PossibleBeats[nNotes].OrderBy(b => UnityEngine.Random.value).First();
            group = group.OrderBy(b => UnityEngine.Random.value).ToArray();

            return new ZMarch(group);
        }

        // [Command("test")]
        private static int[] GetGroup(int nNotes, int i)
        {
            // for the first 3 notes, ignore i, there's only one possible combination.
            if (nNotes == 1) return new int[] { 1 };
            if (nNotes == 2) return new int[] { 2, 2 };
            if (nNotes == 3) return new int[] { 4, 4, 2 };

            // otherwise go bonkers
            int[] group = new int[nNotes];

            if (i == 0)
            {
                group[0] = group[1] = (1 << nNotes - 1);
                for (int gi = 2; gi < nNotes; gi++)
                {
                    group[gi] = 1 << nNotes - gi;
                }
                return group;
            }

            if (i == 1)
            {
                group[0] = group[1] = group[2] = group[3] = 1 << nNotes - 2;
                for (int gi = 4; gi < nNotes; gi++) // won't happen if nNotes == 4
                {
                    group[gi] = 1 << nNotes - (gi);
                }
                return group;
            }

            if (i > 1)
            {
                group[0] = group[1] = 1 << nNotes - 2;
                group[nNotes - 1] = 2;

                for (int j = 2; j < i; j++)
                {
                    group[j] = 1 << (nNotes - j - 1);
                }

                for (int j = i; j < i + 3; j++)
                {
                    group[j] = 1 << nNotes - i - 1;
                }

                for (int j = i + 3; j < nNotes - 1; j++) // go up until the last
                {
                    group[j] = 1 << nNotes - j;
                }
            }

            return group;
        }

        [Command("z.generateBeatNotes")]
        public static string TestGroupGenerator(int nNotes, int i)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            double ms = sw.Elapsed.TotalMilliseconds;
            sw.Stop();
            var group = GetGroup(nNotes, i);
            float sum = 0f;
            string s = "[";
            for (int j = 0; j < group.Length; j++)
            {
                s += group[j] + ", ";
                sum += 1f / group[j];
            }
            s += "] ";
            s += sum;

            s += " ms: " + ms;

            return s;
        }

        /*
         * Fills a buffer of notes within a bar lazily.
         * First note = Random whole, half, quarter, ...
         * Second note = Random note that is < Total bar time - time consumed until now.
         * Third note = ...
         * N note = ...
         * */
        [Command("procrastinator")]
        public static ZMarch GenerateProcrastinatorBeat(int BPM, // beats per minute
                                                      int notesInBar, // upper number of time signature
                                                      int noteType, // lower number of time signature
                                                      Vector2Int noteRange)
        {
            float barTime = GetBarTime(BPM, notesInBar, noteType);

            // list of whole, half, quarter beats that fills the bar
            List<int> beatTypes = new List<int>();
            float totalTime = 0f;
            float targetTime = barTime;
            while (totalTime < targetTime)
            {
                int attempts = 0;
                float randomTime = float.MaxValue;
                int randomNote = -1;
                while (totalTime + randomTime > targetTime)
                {
                    randomNote = 1 << UnityEngine.Random.Range(noteRange.x, noteRange.y);
                    randomTime = GetBeatType(BPM, randomNote);
                    attempts++;

                    if (attempts > 512) throw new System.Exception("Infinite loop.");
                }

                totalTime += randomTime;
                beatTypes.Add(randomNote);
            }

            return new ZMarch(beatTypes.ToArray());
        }

    }

    [System.Serializable]
    public class ZMarch
    {
        public static ZMarch Empty
        {
            get
            {
                return new ZMarch(new int[0]);
            }
        }


        //public float[] Beats;
        public int[] Beats;
        public int Size
        {
            get { return Beats.Length; }
        }

        public int Cursor;
        public int CurrentNoteIndex { get { return Cursor; } }
        public int CurrentNoteType
        {
            get { return Beats[Cursor]; }
        }

        public bool IsEmpty { get => Beats.Length == 0; }

        public ZMarch() { }

        public ZMarch(int[] noteTypes)
        {
            Cursor = 0;
            Beats = noteTypes;
        }

        public ZMarch(ZMarch m)
        {
            Beats = new int[m.Beats.Length];
            Cursor = m.Cursor;
            Array.Copy(m.Beats, Beats, m.Beats.Length);
        }

        // returns the current beat type
        public int Advance(out bool beatEnded)
        {
            int beatType = Beats[Cursor];
            Cursor = (Cursor + 1) % Beats.Length;
            beatEnded = Cursor == 0;
            return beatType;
        }

        public override string ToString()
        {
            string s = "";
            s += "Beats: [";
            foreach (var b in Beats)
            {
                s += b + ", ";
            }
            s += "] \n";
            s += "Size: " + Size + "\n";
            s += "Current Note: " + CurrentNoteIndex;
            return s;
        }
    }

}
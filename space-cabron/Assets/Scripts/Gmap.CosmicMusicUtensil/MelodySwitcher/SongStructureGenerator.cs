using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class SongStructureGenerator 
    {
        StringReferencePool possiblePatterns;
        MelodySwitcher melodySwitcher;

        public SongStructureGenerator(
            MelodySwitcher melodySwitcher,
            StringReferencePool stringReferencePool
        ) {
            this.possiblePatterns = stringReferencePool;
            this.melodySwitcher = melodySwitcher;
        }

        public void Generate() {
            int nPatterns = 1;
            string finalSong = "";
            for (int i = 0; i < nPatterns; i++) {
                string pattern = possiblePatterns.GetNext().Value;
                List<int> melodyIndexes = new List<int>();
                Dictionary<char, int> patternCharacterToMelodyIndex = new Dictionary<char, int>();
                foreach (char c in pattern) {
                    if (patternCharacterToMelodyIndex.ContainsKey(c))
                        continue;

                    int melodyIndex = c - 'A'; // UnityEngine.Random.Range(0, MelodySwitcher.MAX_MELODIES);
                    if (melodyIndexes.Contains(melodyIndex))
                        melodyIndex = (melodyIndex + 1) % MelodySwitcher.MAX_MELODIES;

                    patternCharacterToMelodyIndex[c] = melodyIndex;
                    melodyIndexes.Add(melodyIndex);
                }

                string hyperMeasureStructure = "";
                foreach (char c in pattern) {
                    hyperMeasureStructure += (patternCharacterToMelodyIndex[c] + 1).ToString();
                }

                string sectionStructure = "";

                int count = Mathf.RoundToInt(Mathf.Pow(2, UnityEngine.Random.Range(1, 4)));
                count = 4;
                for (int j = 0; j < count; j++) {
                     sectionStructure += hyperMeasureStructure;
                }

                finalSong += sectionStructure;
            }
            this.melodySwitcher.Structure = finalSong;
        }
    }
}
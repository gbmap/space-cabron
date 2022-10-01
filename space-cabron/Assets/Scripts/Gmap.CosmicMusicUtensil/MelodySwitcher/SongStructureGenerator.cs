using System.Collections.Generic;
using System.Linq;
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
            int nPatterns = 2;
            string finalSong = "";
            for (int i = 0; i < nPatterns; i++) {
                string pattern = possiblePatterns.GetNext().Value;
                // List<int> melodyIndexes = new List<int>();
                Dictionary<char, int> patternCharacterToMelodyIndex = new Dictionary<char, int>();

                var melodyIndexes = Enumerable.Range(0, MelodySwitcher.MAX_MELODIES).OrderBy(x => Random.value).ToList();

                int k = 0;
                foreach (char c in pattern) {
                    if (patternCharacterToMelodyIndex.ContainsKey(c))
                        continue;

                    int melodyIndex = c - 'A'; 

                    if (!patternCharacterToMelodyIndex.ContainsKey(c)) {
                        patternCharacterToMelodyIndex.Add(c, melodyIndexes[k++]);
                    }
                }

                string hyperMeasureStructure = "";
                foreach (char c in pattern) {
                    hyperMeasureStructure += (patternCharacterToMelodyIndex[c] + 1).ToString();
                }

                string sectionStructure = "";

                int count = Mathf.RoundToInt(Mathf.Pow(2, UnityEngine.Random.Range(1, 4)));
                count = 2;
                for (int j = 0; j < count; j++) {
                     sectionStructure += hyperMeasureStructure;
                }

                finalSong += sectionStructure;
            }
            this.melodySwitcher.Structure = finalSong;
        }
    }
}
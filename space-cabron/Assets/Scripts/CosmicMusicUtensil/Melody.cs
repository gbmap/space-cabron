using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gmap.CosmicMusicUtensil
{
    /*
    Example melody:
        "c4/8;d3/8;c3/4;ds3/8
    */
    [System.Serializable]
    public class Melody
    {
        [SerializeField]
        string notation;
        string [] Notes;
        string[] Tones;
        int [] Octaves;
        int [] Intervals;

        public string Notation { get { return notation; } }

        public Melody(string melody)
        {
            notation = melody;
            Update(melody);
        }

        public void Update()
        {
            Update(Notation);
        }

        public Note GetNote(int i) {
            i = Bar.MathMod(i, Notes.Length);
            return new Note(
                Note.FromString(Tones[i].Split('/')[0]),    
                Intervals[i],
                Octaves[i]
            );
        }

        public override string ToString()
        {
            string notation = "";
            for (int i = 0; i < Notes.Length; i++)
            {
                notation += Notes;
                if (i < Notes.Length - 1)
                    notation += ";";
            }
            return notation;
        }

        private void Update(string melody)
        {
            try
            {
                Notes = melody.Split(';');
                Tones = Notes.Select(n => Regex.Replace(n.Split('/')[0], "[0-9]", "")).ToArray();
                Octaves = Notes.Select(n => int.Parse(Regex.Replace(n.Split('/')[0], "[A-z]#?", ""))).ToArray();
                Intervals = Notes.Select(n => int.Parse(n.Split('/')[1])).ToArray();
            }
            catch {}
        }

    }
}


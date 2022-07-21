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
                Tones = Notes.Select(n => MelodyInterpreter.ExtractTone(n)).ToArray();
                Octaves = Notes.Select(n => MelodyInterpreter.ExtractOctave(n)).ToArray();
                Intervals = Notes.Select(n => MelodyInterpreter.ExtractInterval(n)).ToArray();
            }
            catch (System.Exception ex) {
                Debug.LogWarning(ex.Message);
            }
        }

    }

    public class MelodyInterpreter
    {
        public static string[] ExtractNotes(string notation)
        {
            return notation.Split(";");
        }

        public static string[] ExtractTones(string[] notes)
        {
            return notes.Select(n => ExtractTone(n)).ToArray();
        }

        public static string[] ExtractTones(string notation)
        {
            return ExtractTones(ExtractNotes(notation));
        }

        public static string ExtractTone(string note)
        {
            return Regex.Replace(note.Split('/')[0], "[0-9]", "");
        }

        public static int ExtractOctave(string note)
        {
            return int.Parse(Regex.Replace(note.Split('/')[0], "[A-z]#?", ""));
        }
        
        public static int ExtractInterval(string note)
        {
            return int.Parse(note.Split('/')[1]);
        }

        public static string GenerateNote(string tone, int interval, int octave)
        {
            return string.Format("{0}{1}/{2}", tone, octave, interval);
        }
    }
}


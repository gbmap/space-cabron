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
        Note[] NotesArray;

        public string Notation { get { return notation; } }
        public int Length { get { return Notes.Length; } }
        public bool IsEmpty { get { return Notes.Length == 0; } }

        public Melody(Melody m)
        {
            Update(m.notation);
        }

        public Melody(string melody)
        {
            Update(melody);
        }

        public Melody(Note[] notes)
        {
            Update(notes);
        }

        private void Update(string melody)
        {
            if (string.IsNullOrEmpty(melody))
            {
                Notes = new string[0];
                return;
            }

            try
            {
                notation = melody;
                Notes = melody.Split(';');
                if (Notes.Length == 0)
                    return;

                Tones = Notes.Select(n => MelodyInterpreter.ExtractTone(n)).ToArray();
                Octaves = Notes.Select(n => MelodyInterpreter.ExtractOctave(n)).ToArray();
                Intervals = Notes.Select(n => MelodyInterpreter.ExtractInterval(n)).ToArray();
                NotesArray = Enumerable.Range(0, Notes.Length).Select(i => new Note(Note.FromString(Tones[i]), Intervals[i], Octaves[i])).ToArray();
            }
            catch (System.Exception ex) {
                Debug.LogWarning(ex.Message);
            }
        }

        private void Update(Note[] notes)
        {
            Update(string.Join(";", notes.Select(n => n.AsString())));
        }

        public Note GetNote(int i) {
            return NotesArray[Bar.MathMod(i, NotesArray.Length)];
        }

        public void SetNote(Note n, int i){
            if (n == null || i < 0 || i >= Notes.Length)
                return;

            NotesArray[i] = n;
            Update(NotesArray);
        }

        public string AsString()
        {
            return string.Join(";", NotesArray.Select(n=>n.AsString()));
        }

        public void Transpose(int interval)
        {
            for (int i = 0; i < Notes.Length; i++)
                NotesArray[i].Transpose(interval);
            Update(NotesArray);
        }

        public static Melody Transpose(Melody m, int steps)
        {
            Melody m2 = new Melody(m.NotesArray);
            m2.Transpose(steps);
            return m2;
        }

        public static Melody operator+(Melody a, Melody b)
        {
            if (a.IsEmpty && b.IsEmpty)
                return a;
            else if (a.IsEmpty)
                return b;
            else if (b.IsEmpty)
                return a;

            return new Melody(a.AsString() + ";" + b.AsString());
        }

        public static Melody operator+(Melody a, string b)
        {
            return new Melody(a.AsString() + ";" + b);
        }

        public static Melody operator*(Melody a, int n)
        {
            Melody m = new Melody(string.Empty);
            for (int i = 0; i < n; i++)
                m += a;
            return m;
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


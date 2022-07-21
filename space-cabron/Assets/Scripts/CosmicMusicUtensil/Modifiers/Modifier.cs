using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil 
{
    public interface MelodyModifier
    {
        public enum EType
        {
            BreakNote,
            ShiftNote
        }
        Melody Apply(Melody melody);
        EType Type { get; }
    }

    public abstract class MelodyModifierBase : MelodyModifier
    {
        public abstract MelodyModifier.EType Type { get; }

        public class ChewedNote
        {
            public string[] Notes;
            public string Tone;
            public int Octave;
            public int Interval;
            public int NoteIndex;

            public static ChewedNote ParseRandomNote(Melody melody)
            {
                string[] notes = MelodyInterpreter.ExtractNotes(melody.Notation);
                int noteIndex = Random.Range(0, notes.Length);
                string note = notes[noteIndex];
                string tone = MelodyInterpreter.ExtractTone(note);
                int interval = MelodyInterpreter.ExtractInterval(note);
                int octave = MelodyInterpreter.ExtractOctave(note);
                return new ChewedNote
                {
                    Notes = notes,
                    Tone = tone,
                    Octave = octave,
                    Interval = interval,
                    NoteIndex = noteIndex
                };
            }

            public string ReplaceNote(string newNote)
            {
                Notes[NoteIndex] = newNote;
                string notation = string.Join(";", Notes);
                return notation;
            }

            public string ReplaceNote(string newNote, int interval, int octave)
            {
                return ReplaceNote(
                    MelodyInterpreter.GenerateNote(newNote, interval, octave)
                );
            }
        }

        public abstract Melody Apply(Melody melody);
    }

    public class BreakNoteModifier : MelodyModifierBase
    {
        public int NumberOfNotes = 2;

        public override MelodyModifier.EType Type => MelodyModifier.EType.BreakNote;

        public override Melody Apply(Melody melody)
        {
            ChewedNote n = ChewedNote.ParseRandomNote(melody);
            n.Interval *= NumberOfNotes;
            
            string newNotes = "";
            for (int i = 0; i < NumberOfNotes; i++)
            {
                newNotes += MelodyInterpreter.GenerateNote(n.Tone, n.Interval, n.Octave);
                if (i < NumberOfNotes - 1)
                    newNotes += ";";
            }
            return new Melody(n.ReplaceNote(newNotes));
        }
    }

    public class ShiftNoteModifier : MelodyModifierBase
    {
        public int Step = 1;

        public override MelodyModifier.EType Type => MelodyModifier.EType.ShiftNote;

        public override Melody Apply(Melody melody)
        {
            ChewedNote n = ChewedNote.ParseRandomNote(melody);
            ENote note = Note.OffsetNote(
                Note.FromString(n.Tone), 
                Step
            );
            int octaves = Step / 12;
            n.Octave += octaves;

            string newTone = Note.ToString(note);
            return new Melody(n.ReplaceNote(newTone, n.Interval, n.Octave));
        }
    }
}
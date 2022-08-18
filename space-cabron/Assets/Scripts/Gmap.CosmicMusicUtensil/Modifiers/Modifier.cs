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
            ShiftNote,
            TransposeMelody
        }
        Melody Apply(Melody melody);
        EType Type { get; }
    }

    /*
        Melody modifiers modify melodies permanently, changing their notation.
        This is different from Improvisations which are applied temporarily to a
        note.
    */
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

    /*
        Breaks a note in melody, permanently.
    */
    public class BreakMelodyNoteModifier : MelodyModifierBase
    {
        public int NumberOfNotes = 2;

        public override MelodyModifier.EType Type => MelodyModifier.EType.BreakNote;

        public BreakMelodyNoteModifier(int numberOfNotes)
        {
            NumberOfNotes = numberOfNotes;
        }

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

    public class ShiftNoteMelodyModifier : MelodyModifierBase
    {
        public int Steps = 1;

        public override MelodyModifier.EType Type => MelodyModifier.EType.ShiftNote;

        public ShiftNoteMelodyModifier(int steps)
        {
            Steps = steps;
        }

        public override Melody Apply(Melody melody)
        {
            // ChewedNote n = ChewedNote.ParseRandomNote(melody);
            int index = Random.Range(0, melody.Length);
            Melody m2 = new Melody(melody);
            Note n = m2.GetNote(index);

            if (melody.Scale != null)
                n.Tone = melody.Scale.GetNote(melody.Root, Steps);
            else
                n.TransposeWrapped(Steps);
            m2.SetNote(n, index);
            return m2;
        }
    }

    public class TransposeMelodyModifier : MelodyModifierBase
    {
        public override MelodyModifier.EType Type => MelodyModifier.EType.TransposeMelody;
        public int EveryNthPlay = 0;
        public int Steps = 1;

        public TransposeMelodyModifier(int everyNthPlay, int steps)
        {
            EveryNthPlay = Mathf.Max(1, everyNthPlay);
            Steps = steps;
        }

        public override Melody Apply(Melody melody)
        {
            return (melody*(EveryNthPlay-1)) + Melody.Transpose(melody, Steps);
        }
    }
}
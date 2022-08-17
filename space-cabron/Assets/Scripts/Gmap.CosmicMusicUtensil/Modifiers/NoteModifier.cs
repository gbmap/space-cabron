namespace Gmap.CosmicMusicUtensil
{
    public interface NoteModifier
    {
        Note Modify(Note note);
    }

    public class NullNoteModifier : NoteModifier
    {
        public Note Modify(Note note)
        {
            return note;
        }
    }

    public class MuteNoteModifier : NoteModifier
    {
        public Note Modify(Note note)
        {
            note.Tone = ENote.None;
            return note;
        }
    }

    public class TransposeNoteModifier : NoteModifier
    {
        public int Steps;
        public TransposeNoteModifier(int steps)
        {
            Steps = steps;
        }

        public Note Modify(Note note)
        {
            return Note.TransposeWrapped(note, Steps);
        }
    }

    public class CompositeNoteModifier : NoteModifier
    {
        NoteModifier[] Modifiers;
        public CompositeNoteModifier(params NoteModifier[] modifiers)
        {
            Modifiers = modifiers;
        }

        public Note Modify(Note note)
        {
            foreach (NoteModifier m in Modifiers)
                note = m.Modify(note);
            return note;
        }
    }

    public class IncreaseIntervalNoteModifier : NoteModifier
    {
        public int TimesToBreak {get; private set; }
        public IncreaseIntervalNoteModifier(int timesToBreak)
        {
            TimesToBreak = timesToBreak;
        }
        
        public Note Modify(Note note)
        {
            note.Interval*=(TimesToBreak+1);
            return note;
        }
    }

}
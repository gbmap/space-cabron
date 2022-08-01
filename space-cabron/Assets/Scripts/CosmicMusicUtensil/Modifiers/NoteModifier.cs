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
            return Note.Transpose(note, Steps);
        }
    }

    public class BreakNoteModifier : NoteModifier
    {
        public int TimesToBreak {get; private set; }
        public BreakNoteModifier(int timesToBreak)
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
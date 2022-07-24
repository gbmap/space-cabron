using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class Improviser 
    {
        List<Improvisation> improvisations = new List<Improvisation>();

        public Note[] Improvise(Melody melody, int barIndex, Note note, int noteIndex)
        {
            Note[] notes = {note};
            foreach (Improvisation i in improvisations)
                notes = i.Apply(melody, barIndex, notes, noteIndex);
            return notes;
        }

        public void AddImprovisation(Improvisation improvisation)
        {
            if (improvisation == null)
                throw new System.ArgumentNullException("Improvisation is null.");
            improvisations.Add(improvisation);
        }

        public void RemoveImprovisation(Improvisation improvisation)
        {
            improvisations.Remove(improvisation);
        }
    }
}
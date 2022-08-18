using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class Improviser 
    {
        private Dictionary<Improvisation, int> improvisationTimers 
            = new Dictionary<Improvisation, int>();
        List<Improvisation> improvisations = new List<Improvisation>();

        public int NumberOfImprovisations => improvisations.Count;

        Improvisation[] toRemove = new Improvisation[10];

        ITurntable Turntable;
        System.Action<OnImprovisationArgs> OnImprovisationRemoved;

        public Improviser(
            ITurntable turntable=null,
            System.Action<OnImprovisationArgs> OnRemoved=null
        ) {
            Turntable = turntable;
        }

        public Note[] Improvise(Melody melody, int barIndex, Note note, int noteIndex, bool reduceCounter=true)
        {
            Note[] notes = {note};
            int removeCounter = 0;
            foreach (Improvisation i in improvisations)
            {
                if (reduceCounter)
                    removeCounter = ReduceCounter(melody, barIndex, noteIndex, notes, removeCounter, i);
                notes = i.Apply(melody, barIndex, notes, noteIndex);
            }

            for (int i = 0; i < removeCounter; i++)
                RemoveImprovisation(toRemove[i]);

            return notes;
        }

        private int ReduceCounter(Melody melody, int barIndex, int noteIndex, Note[] notes, int removeCounter, Improvisation i)
        {
            if (i.ShouldApply(melody, barIndex, notes, noteIndex) && improvisationTimers.ContainsKey(i))
            {
                improvisationTimers[i]--;
                if (improvisationTimers[i] == 0)
                {
                    improvisationTimers.Remove(i);
                    toRemove[removeCounter++] = i;
                }
            }

            return removeCounter;
        }

        public void AddImprovisation(Improvisation improvisation)
        {
            if (improvisation == null)
                throw new System.ArgumentNullException("Improvisation is null.");
            improvisations.Add(improvisation);
        }

        public void AddImprovisation(Improvisation improvisation, int timer)
        {
            AddImprovisation(improvisation);
            improvisationTimers[improvisation] = timer;
        }

        public void RemoveImprovisation(Improvisation improvisation)
        {
            Turntable?.OnImprovisationRemoved?.Invoke(new OnImprovisationArgs{
                Improvisation = improvisation,
                Life = 0,
                Turntable = Turntable
            });
            improvisations.Remove(improvisation);
        }
    }
}
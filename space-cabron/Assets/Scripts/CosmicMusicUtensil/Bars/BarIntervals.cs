using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public interface IBarIntervals
    {
        bool IsValid { get; }
        float GetNoteTime(int bpm, int i);
        float GetBarTime(int bpm);
    }

    /// <summary>
    /// Describes the number and type of notes in a bar.
    /// For example, {4,4,4,4} = {quarter, quarter, quarter, quarter}
    /// with no tone attached to it. Represents the rhythmic structure
    /// of a bar.
    /// </summary>
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Bar")]
    public class BarIntervals : ScriptableObject, IBarIntervals
    {
        public int TimeSignatureCount = 4;
        public int TimeSignatureType = 4;

        public int[] Notes = { 4, 4, 4, 4};

        public virtual bool IsValid
        {
            get 
            {
                float noteSum = Notes.Sum(x=>1f/x);
                float timeSignature = ((float)TimeSignatureCount)/TimeSignatureType;
                return Mathf.Approximately(noteSum, timeSignature);
            }
        }

        public virtual float GetNoteTime(int bpm, int i)
        {
            i = i%Mathf.Min(TimeSignatureCount, Notes.Length);
            return (1f/Notes[i])*GetBarTime(bpm);
        }

        public virtual float GetBarTime(int bpm)
        {
            float bps = ((float)bpm)/60;
            return 1f/bps * ((float)TimeSignatureCount)/TimeSignatureType;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap.Debug
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class DebugTurntableInfo : MonoBehaviour
    {
        ITurntable turntableBehaviour;

        void Awake() 
        {
            turntableBehaviour = GetComponent<ITurntable>();
        }


        void OnGUI()
        {
            GUILayout.Label($"Note Index: {turntableBehaviour.NoteIndex}");
            GUILayout.Label($"Bar Index: {turntableBehaviour.BarIndex}");

            var melody = turntableBehaviour.Melody;
            var improviser = turntableBehaviour.Improviser;

            List<Note> notes = new List<Note>();
            string m = Enumerable.Range(0,melody.Length).SelectMany(i=>improviser.Improvise(melody, turntableBehaviour.BarIndex, melody.GetNote(i), i)).Select(n=>n.AsString()).Aggregate((a,b)=>a+";"+b);
            GUILayout.Label($"Melody: {m}");
        }
    }
}
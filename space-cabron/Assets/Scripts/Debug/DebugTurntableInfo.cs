using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using SpaceCabron.Scoreboard;
using UnityEngine;

namespace Gmap.Debug
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class DebugTurntableInfo : MonoBehaviour
    {
        TurntableBehaviour turntableBehaviour;

        Score score;
        Score Score
        {
            get {
                if (score == null)
                    score = FindObjectOfType<Score>();
                return score;
            }
        }

        void Awake() 
        {
            turntableBehaviour = GetComponent<TurntableBehaviour>();
        }

        void OnGUI()
        {
            GUILayout.Label($"Note Index: {turntableBehaviour.NoteIndex}");
            GUILayout.Label($"Bar Index: {turntableBehaviour.BarIndex}");
            var melody = turntableBehaviour.Melody;
            var improviser = turntableBehaviour.Improviser;

            List<Note> notes = new List<Note>();
            string m = Enumerable.Range(0,melody.Length)
                                 .SelectMany(i=>improviser.Improvise(melody, turntableBehaviour.BarIndex, melody.GetNote(i), i))
                                 .Select(n=>n.AsString())
                                 .Aggregate((a,b)=>a+";"+b);
            GUILayout.Label($"Melody: {m}");
            GUILayout.Label($"{(turntableBehaviour.Turntable as Turntable).DebugString()}");
            GUILayout.Label("======");

            GUILayout.Label($"Last Note: {turntableBehaviour.Melody.GetNote(turntableBehaviour.NoteIndex - 1).ToString()}");
            GUILayout.Label($"Melody Root: {turntableBehaviour.Melody.Root}");
            GUILayout.Label($"Scale: {turntableBehaviour.Melody.Scale}");
            GUILayout.Label($"Current Level: {LevelLoader.CurrentLevelConfiguration}");
            if (Score != null)
                GUILayout.Label($"Current score: {Score.CurrentScore}");

            if (LevelLoader.CurrentLevelConfiguration is LevelConfiguration levelConfiguration)
            {
                GUILayout.Label($"Target Score: {levelConfiguration.Gameplay.ScoreThreshold}");
            }

            GUILayout.Label($"======");
            EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
            if (spawner)
            {
                ITurntable t = spawner.GetComponentInChildren<ITurntable>();
                if (t != null)
                    GUILayout.Label($"Enemy Spawner BPM: {t.BPM}");
            }

        }
    }
}
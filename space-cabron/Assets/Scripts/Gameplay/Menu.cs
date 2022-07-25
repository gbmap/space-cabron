using System;
using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class Menu : MonoBehaviour
    {
        public LevelConfiguration LevelConfiguration;

        string _customMelody;

        void Awake()
        {
            if (LevelConfiguration != null)
            {
                LevelConfiguration clone = ScriptableObject.CreateInstance<LevelConfiguration>();
                clone.Background = LevelConfiguration.Background;
                clone.Gameplay = LevelConfiguration.Gameplay;
                clone.Melody = LevelConfiguration.Melody;
                LevelConfiguration = clone;
            }
            else
                throw new System.Exception("Level Configuration can't be null.");
        }


        public void SelectRandomMelody()
        {
            StartGame(LevelConfiguration.Melody.PossibleStartingMelodies.GetNext().Value);
        }


        public void OnCustomMelodyTextBarChanged(string text)
        {
            _customMelody = text;
        }

        public void OnCustomMelodyConfirmed()
        {
            try
            {
                Melody m = new Melody(_customMelody);
                if (m.IsEmpty)
                    throw new System.Exception("Melody is empty.");
                StartGame(_customMelody);
            }
            catch
            {
                // ... 
            }
        }

        private void SelectMelody(string notation)
        {
            LevelConfiguration.Melody.StartingMelody = new Melody(notation);
        }

        private void StartGame(string melody)
        {
            SelectMelody(melody);
            StartGame();
        }

        private void StartGame()
        {
            LevelLoader.Load(LevelConfiguration);
        }
    }
}
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
                LevelConfiguration = LevelConfiguration.Clone();
            }
            else
                throw new System.Exception("Level Configuration can't be null.");
        }


        public void SelectRandomMelody()
        {
            MelodyConfiguration melodyConfig = LevelConfiguration.GetMelodyConfigurationByTag("Player");
            StartGame(melodyConfig.PossibleStartingMelodies.GetNext().Value);
        }

        public void OnCustomMelodyTextBarChanged(string text)
        {
            _customMelody = text;
        }

        public void OnCustomMelodyConfirmed()
        {
            try
            {
                Melody m = new Melody(_customMelody.ToLower());
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
            var melodyConfig = LevelConfiguration.GetMelodyConfigurationByTag("Player");
            melodyConfig.StartingMelody = new Melody(notation);
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
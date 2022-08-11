using System;
using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
using SpaceCabron.Gameplay;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class Menu : MonoBehaviour
    {
        public LevelConfiguration LevelConfiguration;
        public IntReference Score;

        string _customMelody;

        void Awake()
        {
            if (LevelConfiguration != null)
            {
                LevelConfiguration = LevelConfiguration.Clone() as LevelConfiguration;
            }
            else
                throw new System.Exception("Level Configuration can't be null.");
        }


        public void SelectRandomMelody()
        {
            StartGame();
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

                ScriptableFixedMelodyFactory f = ScriptableObject.CreateInstance<ScriptableFixedMelodyFactory>();
                f.Notation = _customMelody.ToLower();
                LevelConfiguration.GetInstrumentConfigurationByTag("Player").MelodyFactory = f;
                StartGame();
            }
            catch
            {
                // ... 
            }
        }

        private void StartGame()
        {
            Score.Value = 0;
            LevelLoader.Load(LevelConfiguration);
        }
    }
}
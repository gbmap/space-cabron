using Gmap.ScriptableReferences;
using SpaceCabron.Gameplay;
using SpaceCabron.Gameplay.Multiplayer;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class Menu : MonoBehaviour
    {
        public enum EGameMode
        {
            Arcade,
            BossRun
        }

        public LevelList ArcadeModeLevelList;
        public LevelList BossRunLevelList;
        public IntReference Score;
        public EGameMode GameMode = EGameMode.Arcade;

        string _customMelody;

        public void SetNumberOfPlayers(int n)
        {
            MultiplayerManager.PlayerCount = n;
        }

        public void SetGameMode(int mode)
        {
            GameMode = (EGameMode)mode;
        }

        public void StartGame()
        {
            Score.Value = 0;
            LevelLoader.Load(GetLevelList(GameMode).List[0]);
        }

        public void ExitGame() 
        {
            Application.Quit(0);
        }

        private LevelList GetLevelList(EGameMode mode) {
            switch (mode) {
                case EGameMode.Arcade:
                    return ArcadeModeLevelList;
                case EGameMode.BossRun:
                    return BossRunLevelList;
                default:
                    return ArcadeModeLevelList;
            }
        }
        
    }
}
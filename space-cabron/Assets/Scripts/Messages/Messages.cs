using UnityEngine;

namespace SpaceCabron.Messages
{
    public class MsgOnScoreChanged
    {
        public int TotalScore;
        public int Score;
        public MsgOnScoreChanged(int score, int totalScore)
        {
            Score = score;
            TotalScore = totalScore;
        }
    }

    public class MsgIncreaseScore
    {
        public int Value;
        public MsgIncreaseScore(int value)
        {
            Value = value;
        }
    }

    public class MsgLevelWon {}
    public class MsgPauseGame 
    {
        public bool Value;
        public MsgPauseGame(bool v)
        {
            Value = v;
        }
    }

    public class MsgLevelStartedLoading {}
    public class MsgLevelFinishedLoading {}

    public class MsgSpawnDrone 
    {
        public enum EDroneType
        {
            Random,
            Melody,
            EveryN
        }
        public EDroneType DroneType;
        public GameObject Player;

        public System.Action<GameObject> OnSpawned;
    }

    public class MsgOnDroneSpawned
    {
        public GameObject Drone;
    }

    public class MsgSpawnPlayer
    {
        public GameObject TargetPosition;
        public Vector3 Position;
        public System.Action<GameObject> OnSpawned;
    }

    public class MsgSpawnPlayerChip 
    {
        public Vector3 Position;
    }

    public class MsgGameOver {}
}
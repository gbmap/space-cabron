using UnityEngine;

namespace SpaceCabron.Messages
{
    public class MsgLevelStartedLoading {}
    public class MsgLevelFinishedLoading {}

    public class MsgOnRetry {}

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

    public class MsgSpawnDrone 
    {
        public enum EDroneType
        {
            Random,
            Melody,
            EveryN,
            Any
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
        public int PlayerIndex = 0;
        public bool IsRespawn = false;
        public GameObject TargetPosition;
        public Vector3 Position;
        public System.Action<GameObject> OnSpawned;
    }

    public class MsgOnPlayerSpawned
    {
        public GameObject Player;
    }

    public class MsgSpawnPlayerChip 
    {
        public Vector3 Position;
    }

    public class MsgGameOver {}

    public class MsgPlayerJoined {}
    public class MsgPlayerLeft {}

    public class MsgOnNotePlayedOutOfTime
    {
        public int PlayerIndex;
    }

    public class MsgOnNotePlayedInTime
    {
        public int PlayerIndex;
    }

    public class MsgOnWrongBulletHit
    {
        public int PlayerIndex;
    }

    public class MsgOnComboBroken 
    {
        public int PlayerIndex;
        public int Combo;
    }

    public class MsgOnComboIncrease {
        public int PlayerIndex;
        public int CurrentCombo;
    }
}
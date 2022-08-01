using UnityEngine;

namespace SpaceCabron.Messages
{
    public class RandomizeBeat {}
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
        public GameObject Player;
    }
}
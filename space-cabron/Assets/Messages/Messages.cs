using UnityEngine;

namespace Gmap.Messages {
    public class RandomizeBeat {}
}

public class Message {}

namespace Gmap.Messages 
{
    public class MsgOnScoreChanged
    {
        public int Score;
        public MsgOnScoreChanged(int score)
        {
            Score = score;
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
}
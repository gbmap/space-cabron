using UnityEngine;

namespace SpaceCabron.Messages {
    public class RandomizeBeat {}
}

public class Message {}

namespace SpaceCabron.Messages 
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
}
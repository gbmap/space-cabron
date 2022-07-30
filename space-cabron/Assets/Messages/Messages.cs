namespace SpaceCabron.Messages
{
    public class RandomizeBeat {}
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
    public class MsgPauseGame 
    {
        public bool Value;
        public MsgPauseGame(bool v)
        {
            Value = v;
        }

    }
}
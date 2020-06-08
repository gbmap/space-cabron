using UnityEngine;

public class BeatMaker : MonoBehaviour
{
    [Range(1, 360)]
    public static int BPM = 120;

    [Range(1, 8)]
    public int MaxSubBeats = 1;

    [Range(1, 8)]
    public int BeatsPerFourBeats = 4;

    [Range(1, 16)]
    public int BeatsInBar = 4;

    public int BeatNumber = 0;

    public Loop Loop { get; private set; }

    public int CurrentBeat
    {
        get; private set;
    }

    public int CurrentBar
    {
        get; private set;
    }

    public int CurrentSection
    {
        get; private set;
    }

    public System.Action OnBeat;
    public System.Action<int> OnBar;
    public System.Action<int> OnSection;
    public System.Action<int> OnNewBeat;

    bool playedBeat;
    bool _playing;

    public void Stop()
    {
        _playing = false;
        CurrentBeat = 0;
        CurrentBar = 0;
        CurrentSection = 0;
    }
    
    public void Pause()
    {
        _playing = false;
    }

    public void Run()
    {
        _playing = true;
        CurrentBeat = 0;
        CurrentBar = 0;
        CurrentSection = 0;
    }

    private const int SectionBarCount = 8;

    float BeatsPerSecond { get { return ((60f/BPM)); } }
    private float CalculateBeatCooldown(float bps)
    {
        return BeatsPerSecond * (4f/BeatsPerFourBeats) / Loop.CurrentBeat.SubBeats;
    }
    
    private void Awake()
    {
        RefreshLoop();
        Run();
    }

    public void RefreshLoop()
    {
        Loop = LoopCreator.Create(BeatsInBar, MaxSubBeats);
        CurrentBeat = 0;
        OnNewBeat?.Invoke(Loop.BeatCount);
    }

    private float t;
    public void Update()
    {
        if (!_playing)
        {
            return;
        }

        float bcd = CalculateBeatCooldown(BeatsPerSecond);
        t = Mathf.Min(bcd, t + Time.deltaTime);
        if (Mathf.Approximately(t, bcd))
        {
            bool endBeat;
            bool endBar = Loop.Advance(out endBeat);
            if (endBar)
            {
                OnBar?.Invoke(CurrentBar);

                if (CurrentBar == SectionBarCount)
                {
                    CurrentBar = 0;
                    CurrentSection++;

                    OnSection?.Invoke(CurrentSection);
                }
                CurrentBar++;
                CurrentBeat = 0;
            }
        
            OnBeat?.Invoke();
            playedBeat = true;
            CurrentBeat++;
            BeatNumber++;
            t = 0f;
        }
    }
}
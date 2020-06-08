using UnityEngine;

public class BeatMaker : MonoBehaviour
{
    [Range(1, 360)]
    public static int BPM = 80;

    [Range(1, 8)]
    public int MaxSubBeats = 1;

    [Range(1, 8)]
    public int BeatsInBar = 4;

    [Range(1, 16)]
    public int NBeats = 4;

    public int BeatNumber = 0;

    public Loop Loop { get; private set; }

    public System.Action OnBeat;
    public System.Action<int> OnBar;
    public System.Action<int> OnSection;
    public System.Action<int> OnNewBeat;

    // TODO: jogar pra dentro do loop creator
    bool _playing;
    public void Stop()
    {
        _playing = false;
        CurrentBeat = 0;
        _currentBar = 0;
        _currentSection = 0;
    }
    
    public void Pause()
    {
        _playing = false;
    }

    public void Run()
    {
        _playing = true;
        CurrentBeat = 0;
        _currentBar = 0;
        _currentSection = 0;
    }

    public int CurrentBeat
    {
        get; private set;
    }

    float _lastBeat;
    int _currentBar;
    int _currentSection;
    bool playedBeat;

    private const int SectionBarCount = 8;

    float BeatsPerSecond { get { return ((60f/BPM)); } }
    private float CalculateBeatCooldown(float bps)
    {
        return BeatsPerSecond * (4f/BeatsInBar) / Loop.CurrentBeat.SubBeats;
    }
    
    private void Awake()
    {
        _lastBeat = Time.time;
        RefreshLoop();
        Run();
    }

    public void RefreshLoop()
    {
        Loop = LoopCreator.Create(NBeats, MaxSubBeats);
        CurrentBeat = 0;
        OnNewBeat?.Invoke(Loop.BeatCount);
    }

    public void Update()
    {
        if (!_playing)
        {
            return;
        }

        float bcd = CalculateBeatCooldown(BeatsPerSecond);
        float p = Time.time % bcd;
        if (p > bcd*0.95 && !playedBeat)
        {
            bool endBeat;
            bool endBar = Loop.Advance(out endBeat);
            if (endBar)
            {
                OnBar?.Invoke(_currentBar);

                if (_currentBar == SectionBarCount)
                {
                    _currentBar = 0;
                    _currentSection++;

                    OnSection?.Invoke(_currentSection);
                }
                _currentBar++;
                CurrentBeat = 0;
            }
        
            OnBeat?.Invoke();
            playedBeat = true;
            CurrentBeat++;
            BeatNumber++;
            _lastBeat = _lastBeat+bcd; // ajustar tempo da nota
        }
        else if (p < bcd*0.95f)
        {
            playedBeat = false;
        }
    }
}
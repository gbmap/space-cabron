using UnityEngine;

public class BeatMaker : MonoBehaviour
{
    [Range(1, 360)]
    public int BPM = 80;

    [Range(1, 8)]
    public int MaxSubBeats = 1;

    [Range(1, 8)]
    public int BeatsInBar = 4;

    public Loop Loop { get; private set; }

    public System.Action OnBeat;
    public System.Action<int> OnBar;
    public System.Action<int> OnSection;

    // TODO: jogar pra dentro do loop creator
    bool _playing;
    public void Stop()
    {
        _playing = false;
        _currentBeat = 0;
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
        _currentBeat = 0;
        _currentBar = 0;
        _currentSection = 0;
    }

    int _currentBeat;
    float _lastBeat;
    int _currentBar;
    int _currentSection;

    private const int SectionBarCount = 8;

    float BeatsPerSecond { get { return (60f/BPM); } }
    private float CalculateBeatCooldown(float bps)
    {
        return BeatsPerSecond / Loop.CurrentBeat.SubBeats;
    }
    
    private void Awake()
    {
        Loop = LoopCreator.Create(BeatsInBar, MaxSubBeats);

        Run();
    }

    public void RefreshLoop()
    {
        Loop = LoopCreator.Create(BeatsInBar, MaxSubBeats);
    }

    public void Update()
    {
        if (!_playing)
        {
            return;
        }

        if (Time.time > _lastBeat + CalculateBeatCooldown(BeatsPerSecond))
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
            }

        
            OnBeat?.Invoke();
            _lastBeat = Time.time;
        }
    }
}
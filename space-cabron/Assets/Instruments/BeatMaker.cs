using UnityEngine;

public class BeatMaker : MonoBehaviour
{
    public int BPM;
    /*public int MaxSubBeats = 1;
    public int MaxInstrumentsInBeat = 1;
    public int BeatsInBar = 4;*/

    public Loop Loop { get; private set; }
    public LoopCreator LoopCreator { get; private set; }

    public System.Action<int[]> OnBeat;
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
        return BeatsPerSecond / Loop.CurrentBeat.subBeats.Length;
    }
    /*
    private void Awake()
    {
        LoopCreator = new LoopCreator()
    }
    */
    public BeatMaker(int[] weights, int maxSubBeats, int maxInstrumentsInBeat, int nBeats, int bpm = 100)
    {
        BPM = bpm;
        LoopCreator = new LoopCreator(weights, nBeats, maxSubBeats, maxInstrumentsInBeat);
        Loop = LoopCreator.Create(nBeats);
    }

    public void RefreshLoop()
    {
        Loop = LoopCreator.Create();
    }

    public void Update(bool control = false)
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

            /*
            EInstrumentAudio audio = loop.GetInstrumentAudio();
            if (audio != EInstrumentAudio.None)
            {
                audioSource.PlayOneShot(instrument.GetAudio(audio));
            }
            */

            int[] notes = Loop.GetInstrumentAudios();
            /*foreach (var audio in notes)
            {
                if (audio == EInstrumentAudio.None) continue;
                //audioSource.PlayOneShot(instrument.GetAudio(audio));
            }*/
            
            OnBeat?.Invoke(notes);
            _lastBeat = Time.time;
        }

        if (control)
        {
            Control();
        }
    }

    private void Control()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) { LoopCreator.MaxSubBeats++; }
            if (Input.GetKeyDown(KeyCode.KeypadMinus)) { LoopCreator.MaxSubBeats--; }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) { LoopCreator.NBeats++; }
            if (Input.GetKeyDown(KeyCode.KeypadMinus)) { LoopCreator.NBeats--; }
        }

        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) { LoopCreator.MaxInstrumentsInBeat++; }
        if (Input.GetKeyDown(KeyCode.KeypadDivide)) { LoopCreator.MaxInstrumentsInBeat--; }
        
        if (Input.GetKeyDown(KeyCode.R)) { Loop = LoopCreator.Create(); }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { LoopCreator.Weights[0]--; }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { LoopCreator.Weights[1]--; }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { LoopCreator.Weights[2]--; }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { LoopCreator.Weights[3]--; }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { LoopCreator.Weights[4]--; }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { LoopCreator.Weights[0]++; }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { LoopCreator.Weights[1]++; }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { LoopCreator.Weights[2]++; }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { LoopCreator.Weights[3]++; }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { LoopCreator.Weights[4]++; }
        }
    }

    public void OnGUI()
    {
        Debug();
    }

    public void Debug()
    {
        GUILayout.Label("Audio: " + Loop.CurrentBeat.CurrentAudio);
        GUILayout.Label("Sub-beat: " + Loop.CurrentBeat.Index);
        GUILayout.Label("Beat: " + Loop.Index);
        GUILayout.Label("Bar: " + _currentBar);
        GUILayout.Label("Section: " + _currentSection);
        GUILayout.Label("===========");
        GUILayout.Label("(- / +) N Beats: " + LoopCreator.NBeats);
        GUILayout.Label("(shft - / +) Max Sub-beats: " + LoopCreator.MaxSubBeats);
        GUILayout.Label("Max Instruments in Beat: " + LoopCreator.MaxInstrumentsInBeat);
        GUILayout.Label("===== WEIGHTS =====");
        for (int i = 0; i < LoopCreator.Weights.Length; i++)
        {
            GUILayout.Label("("+i+") "+((i).ToString() + ": " + LoopCreator.Weights[i]));
        }
    }
}
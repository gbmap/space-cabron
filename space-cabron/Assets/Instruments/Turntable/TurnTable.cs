using UnityEngine;


[System.Serializable]
public class TurnTable : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;

    public int BPM = 80;
    public float Speed = 1f;
    public float DelayFactor { get { return (120f / BPM) * (1f / Speed); } }
    
    [HideInInspector]
    public Loop loop;
    public InstrumentAudioSample instrument;
    
    public System.Action<EInstrumentAudio> OnBeat;
    public System.Action<int> OnBar;
    public System.Action<int> OnSection;

    public int NBeats = 4;

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
        return BeatsPerSecond / loop.CurrentBeat.subBeats.Length;
    }

    private void Awake()
    {
        loop = LoopCreator.Create(NBeats);
        Run();
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
            bool endBar = loop.Advance(out endBeat);
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

            EInstrumentAudio[] audios = loop.GetInstrumentAudios();
            foreach (var audio in audios)
            {
                if (audio == EInstrumentAudio.None) continue;
                audioSource.PlayOneShot(instrument.GetAudio(audio));
            }
            
            OnBeat?.Invoke(audios[0]);
            _lastBeat = Time.time;
        }

        Control();
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
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) { NBeats++; }
            if (Input.GetKeyDown(KeyCode.KeypadMinus)) { NBeats--; }
        }

        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) { LoopCreator.MaxInstrumentsInBeat++; }
        if (Input.GetKeyDown(KeyCode.KeypadDivide)) { LoopCreator.MaxInstrumentsInBeat--; }
        
        if (Input.GetKeyDown(KeyCode.R)) { loop = LoopCreator.Create(NBeats); }

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

    private void OnGUI()
    {
        Debug();
    }

    public void Debug()
    {
        GUILayout.Label("Audio: " + loop.CurrentBeat.CurrentAudio);
        GUILayout.Label("Sub-beat: " + loop.CurrentBeat.Index);
        GUILayout.Label("Beat: " + loop.Index);
        GUILayout.Label("Bar: " + _currentBar);
        GUILayout.Label("Section: " + _currentSection);
        GUILayout.Label("===========");
        GUILayout.Label("(- / +) N Beats: " + NBeats);
        GUILayout.Label("(shft - / +) Max Sub-beats: " + LoopCreator.MaxSubBeats);
        GUILayout.Label("Max Instruments in Beat: " + LoopCreator.MaxInstrumentsInBeat);
        GUILayout.Label("===== WEIGHTS =====");
        for (int i = 0; i < LoopCreator.Weights.Length; i++)
        {
            GUILayout.Label("("+i+") "+((EInstrumentAudio)i).ToString() + ": " + LoopCreator.Weights[i]);
        }
    }
}
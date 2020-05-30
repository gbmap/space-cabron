using System;
using System.Linq;
using UnityEngine;

public class Drum : MonoBehaviour
{
    public AudioSource source;
    public InstrumentAudioSample instrument;
    public BeatMaker tt;

    void Awake()
    {
        tt = new BeatMaker(new int[] { 3, 1, 1, 1, 2 }, 1, 1, 8);
        tt.Run();
    }

    private void OnEnable()
    {
        tt.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        tt.OnBeat -= OnBeat;    
    }

    // Update is called once per frame
    void Update()
    {
        tt.Update();
    }

    private void OnBeat(int[] notes)
    {
        EInstrumentAudio[] samples = notes.Select(n => (EInstrumentAudio)n).ToArray();
        foreach (var sample in samples)
        {
            source.PlayOneShot(instrument.GetAudio(sample));
        }
    }

    private void OnGUI()
    {
        tt.OnGUI();
    }
}

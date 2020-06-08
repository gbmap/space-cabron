using UnityEngine;

public class ZapperzCurtisMarcher : MonoBehaviour
{
    [Range(30, 320)]
    public int BPM = 80;

    [Range(1, 16)]
    public int NotesInBar = 4;

    [Header("1 = whole, 2 = half, 3 = quarter, etc...")]
    [Range(1, 6)]
    public int NoteType = 4;

    public Vector2Int NoteRange = new Vector2Int(1, 4);

    public ZBar bar;

    float t;

    public System.Action<int> OnBeat; // param1 = beat type

    private void Awake()
    {
        GenerateNewBar();
    }

    public void GenerateNewBar()
    {
        bar = ZapperzFootTapping.GenerateProcrastinatorBeat(BPM, NotesInBar, NoteType, NoteRange);
    }

    private void Update()
    {
        float bt = ZapperzFootTapping.GetBeatType(BPM, bar.CurrentBeat);

        t = Mathf.Min(bt, t + Time.deltaTime);
        if (Mathf.Approximately(t, bt))
        {
            OnBeat?.Invoke(bar.CurrentBeat);

            bool ended;
            bar.Advance(out ended);
            t = 0f;
        }
    }

    /*

    private void TestBeatTime1(int bpm, int notesInBar)
    {
        float t = ZapperzFootTapping.GetBarTime1(bpm, notesInBar);
        Debug.LogFormat("Get Bar Time 1 with {0} BPM and {1} notes returned: {2} barTime", bpm, notesInBar, t);
    }

    private void TestBeatTime2(int bpm, int notesInBar, int noteType)
    {
        float t = ZapperzFootTapping.GetBarTime2(bpm, notesInBar, noteType);
        Debug.LogFormat("Get Bar Time 2 with {0} BPM and {1} notes of type {3} returned: {2} barTime", bpm, notesInBar, t, noteType);
    }

    */


}

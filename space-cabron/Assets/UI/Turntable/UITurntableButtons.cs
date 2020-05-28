using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UITurntableButtons : MonoBehaviour
{
    public TurnTable tt;

    private void Start()
    {
        tt.OnBeat += OnBeat;
    }

    private void OnBeat(EInstrumentAudio obj)
    {
        UpdateButtons();
    }

    // Update is called once per frame
    public void UpdateButtons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            int x = i % 16;
            int y = i / 16;
            var c = transform.GetChild(i);
            var img = c.GetComponent<Image>();
            img.color = Color.gray;

            if (x >= tt.loop.Beats.Length)
            {
                img.color = new Color(0.1f, 0.1f, 0.1f);
                continue;
            }

            int t = tt.loop.Index;
            //var s = tt.loop.Beats[x].instrumentAudio.Cast<int>().Select(ss => ss - 1); //((int)tt.loop.Beats[x].instrumentAudio[0])-1;
            int s = -1 + ((int)tt.loop.Beats[x].CurrentAudio);

            if (x == t) img.color = Color.green;
            if (s == y) img.color = Color.white;
        }
        /*for (int i = 0; i < LoopCreator.MAX_LOOPBEATS; i++)
        {
            Beat b = tt.loop.CurrentBeat; ;
            if (i < tt.loop.Beats.Length)
            {
                b = tt.loop.CurrentBeat;
            }
            else
            {
                b = null;
            }

            int t = b != null ? (int)b.instrumentAudio[0] : -100;
            for (int j = 0; j < (int)EInstrumentAudio.HiHat; j++)
            {
                int k = i + (LoopCreator.MAX_LOOPBEATS * j);
                transform.GetChild(k).GetComponent<Image>().color = j == t ?
                    Color.white :
                    (tt.loop.Index) == i ? Color.green : Color.gray;
            }
        }*/
    }

}

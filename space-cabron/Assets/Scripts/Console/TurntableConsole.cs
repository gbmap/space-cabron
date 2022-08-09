using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;
using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
using System;

public class TurntableConsole
{
    private static TurntableBehaviour[] turntable = new TurntableBehaviour[10];
    private static int turntableIndex = 0;
    private static int turntableCount = 0;

    [Command("turntable", "Creates a turntable.")]
    public static void Turntable(int bpm=60)
    {
        Debug.Log($"Created turntable at {turntableCount}");
        turntable[turntableCount++] = CreateTurntable(bpm);
    }

    public static TurntableBehaviour CreateTurntable(int bpm=60)
    {
        var instrumentPrefab = Resources.Load<GameObject>("Instrument");
        var instrument = GameObject.Instantiate(instrumentPrefab);
        
        var turntable = instrument.GetComponent<TurntableBehaviour>();
        turntable.BPMReference = new IntBusReference();
        turntable.BPMReference.Value = bpm;
        return turntable;
    }

    [Command("channel", "Sets the turntable at index <index> to play on channel <channel>.")]
    public static void Channel(int index, int channel)
    {
        if (index >= turntableCount)
        {
            Debug.LogError($"No turntable at index {index}");
            return;
        }
    }

    [Command("melody", "Creates and add a melody to the current turntable.")]
    public static void Melody(int index)
    {
        TurntableBehaviour turntable = GetTurntable(index);
        turntable.SetMelody(
            new RandomMelodyFactory(
                ENote.C, 
                Resources.Load<ScriptableScale>("Scales/Aeolian"), 
                4,
                IntReference.Create(4) as IntReference
            ).GenerateMelody()
        );
    }

    private static TurntableBehaviour GetTurntable(int index)
    {
        if (index < 0 || index >= turntableCount)
            throw new System.IndexOutOfRangeException($"No turntable at index {index}");
        return turntable[index];
    }
}

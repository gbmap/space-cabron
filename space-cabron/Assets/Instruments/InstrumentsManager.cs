using Frictionless;
using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Useful;
using Z;

//[CommandPrefix("instruments.")]
public class InstrumentsManager : Singleton<InstrumentsManager>
{
    [Header("Marchers")]
    [Command("enemy.marchPlayer")]
    public ZMarchPlayer EnemyBeatMaker;
    
    [Command("enemy.march")]
    public ZMarch EnemyMarch
    {
        get { return EnemyBeatMaker.March; }
        set { EnemyBeatMaker.March = value; }
    }

    [Command("player.marchPlayer")]
    public ZMarchPlayer PlayerBeatMaker;

    [Command("player.march")]
    public ZMarch PlayerMarch
    {
        get { return PlayerBeatMaker.March; }
        set { PlayerBeatMaker.March = value; }
    }

    [Header("Melody")]
    [Command("enemy.melodyPlayer")]
    public ZBaseMelodyPlayer EnemySpawnerDrum;

    [Command("player.melodyPlayer")]
    public ZBaseMelodyPlayer PlayerMelodyPlayer;

    [Command("player.melody")]
    public ZMelody PlayerMelody
    {
        get { return PlayerMelodyPlayer.Melody; }
        set { PlayerMelodyPlayer.Melody = value; }
    }

    [Command("enemy.melody")]
    public ZMelody EnemyMelody
    {
        get { return EnemySpawnerDrum.Melody; }
        set { EnemySpawnerDrum.Melody = value; }
    }

    [Header("Instruments")]
    public Synth PlayerSynth;

    public SynthLoopNotes BulletsSynth;

    MessageRouter _router;

    private void Awake()
    {
        LoadCommandsHashTable();
    }

    private void OnEnable()
    {
        _router = ServiceFactory.Instance.Resolve<MessageRouter>();
        _router.AddHandler<MsgOnEnemyHit>(Cb_OnEnemyHit);
        _router.AddHandler<MsgOnWaveEnded>(Cb_OnWaveEnded);
    }

    private void OnDisable()
    {
        _router.RemoveHandler<MsgOnEnemyHit>(Cb_OnEnemyHit);
        _router.RemoveHandler<MsgOnWaveEnded>(Cb_OnWaveEnded);
    }

    private void Cb_OnWaveEnded(MsgOnWaveEnded obj)
    {
        ZMarchPlayer.BPM += 5;
        //if (obj.Wave.Index % 5 != 0) return;
        //EnemySpawnerDrum.UpdateNoteBag(EnemyBeatMaker.NotesInBar, true);
    }

    private void Cb_OnEnemyHit(MsgOnEnemyHit msg)
    {
        BulletsSynth?.Play();
    }

    public ZNoteTypeImproviser GenerateRandomImproviser(ZMarchPlayer marcher)
    {
        return new ZMarchFragmentImproviser(
            marcher.BaseMarch, 
            UnityEngine.Random.Range(1, marcher.BaseMarch.Size),
            UnityEngine.Random.Range(2, 2*10)
        );
    }

    public ZNoteImproviser GenerateRandomImproviser(ZBaseMelodyPlayer melody)
    {
        return new ZMelodyImproviserAltNotes(
            melody.Melody,
            UnityEngine.Random.Range(1, melody.Melody.Length),
            UnityEngine.Random.Range(2, 2 * 10)
        );
    }

    #region CONSOLE

    /////////////////////////////////////
    ///  CONSOLE COMMANDS
    /// 
    [Command("enemy.marcher.refresh")]
    public void RefreshEnemyBeat()
    {
        //EnemyBeatMaker?.GenerateNewBeat();
    }

    [Command("enemy.melody.refresh")]
    public void RefreshEnemyInstruments()
    {
        //EnemySpawnerDrum?.GenerateNewNotes();
    }

    [Command("player.marcher.refresh")]
    public void RefreshPlayerBeat()
    {
        //PlayerBeatMaker?.GenerateNewBeat();
    }

    [Command("player.melody.refresh")]
    public void RefreshPlayerMelody()
    {
        //PlayerMelodyGenerator?.GenerateMelody();
    }

    [Command("testMarcher")]
    private void TestMarcher(string target)
    {
        QuantumConsole.Instance.InvokeCommand(target + ".march { marchGen.fixed 4 }");
        QuantumConsole.Instance.InvokeCommand(target + ".melody { melodyGen.skipAndWalk A Major 4 4 }");
    }

    Dictionary<string, int> commImproviserTypeHashes;

    void LoadCommandsHashTable()
    {
        void InitializeTable(ref Dictionary<string, int> dict, string[] values)
        {
            dict = new Dictionary<string, int>();
            foreach (var v in values)
            {
                dict[v] = v.GetHashCode();
            }
        }

        InitializeTable(ref commImproviserTypeHashes, new string[] { "fragment", "altNotes" });
    }

    [Command("addMarchImprov")]
    public void AddImproviser(ZMarchPlayer marcher, string type, List<int> p)
    {
        AddMarchImproviser(marcher.Improviser, marcher.BaseMarch, type, p.Cast<object>().ToList());
    }

    [Command("addMelodyImprov")]
    public void AddImproviser(ZMelodyPlayerSynth melody, string type, List<int> p)
    {
        AddMelodyImproviser(melody.Improviser, melody.Melody, type, p.Cast<object>().ToList());
    }

    private void AddMelodyImproviser(ZMelodyImproviser imp, ZMelody m, string type, List<object> p)
    {
        if (type == "altNotes")
        {
            imp.Improvisers.Add(new ZMelodyImproviserAltNotes(m, (int)p[0], (int)p[1]));
        }
        else
        {
            throw new Exception("Note improviser type not found");
        }
    }

    private void AddMarchImproviser(ZMarchImproviser imp, ZMarch m, string type, List<object> p)
    {
        if (type == "fragment")
        {
            imp.Improvisers.Add(new ZMarchFragmentImproviser(m, (int)p[0], (int)p[1]));
        }
        else
        {
            throw new Exception("March improviser type not found!");
        }
    }

    #endregion

}

public class ZMarchParser : BasicQcParser<ZMarch>
{
    public override ZMarch Parse(string value)
    {
        if (value.Contains("player"))
        {
            return InstrumentsManager.Instance.PlayerMarch;
        }
        else if (value.Contains("enemy"))
        {
            return InstrumentsManager.Instance.EnemyMarch;
        }
        else
        {
            return ZMarch.Empty;
        }
    }
}

public class ZMarchSerializer : BasicQcSerializer<ZMarch> 
{
    public override string SerializeFormatted(ZMarch value, QuantumTheme theme)
    {
        return value.ToString();
    }
}

public class ZMelodyParser : BasicQcParser<ZMelody>
{
    public override ZMelody Parse(string value)
    {
        if (value.Contains("player"))
        {
            return InstrumentsManager.Instance.PlayerMelody;
        }

        return ZMelody.Empty;
    }
}

public class ZMelodySerializer : BasicQcSerializer<ZMelody>
{
    public override string SerializeFormatted(ZMelody value, QuantumTheme theme)
    {
        return value.ToString();
    }
}

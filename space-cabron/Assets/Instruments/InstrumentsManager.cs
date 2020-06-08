using System;
using Frictionless;
using Useful;

public class InstrumentsManager : Singleton<InstrumentsManager>
{
    public Drum EnemySpawnerDrum;
    public BeatMaker EnemyBeatMaker;
    public Synth PlayerSynth;
    public BeatMaker PlayerBeatMaker;

    public SynthLoopNotes BulletsSynth;

    MessageRouter _router;

    private void Start()
    {
        PlayerSynth.NoteSequencer.StartWithRandomNote();
        EnemySpawnerDrum.StartWithRandomNote();
    }

    private void OnEnable()
    {
        _router = ServiceFactory.Instance.Resolve<MessageRouter>();
        _router.AddHandler<MsgOnEnemyHit>(Cb_OnEnemyHit);
    }

    private void OnDisable()
    {
        _router.RemoveHandler<MsgOnEnemyHit>(Cb_OnEnemyHit);
    }

    private void Cb_OnEnemyHit(MsgOnEnemyHit msg)
    {
        BulletsSynth.Play();
    }
}

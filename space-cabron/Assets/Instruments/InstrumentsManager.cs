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
        EnemySpawnerDrum.StartWithRandomNote();
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
        if (obj.Wave.Index % 5 != 0) return;
        EnemySpawnerDrum.UpdateNoteBag(true);
    }

    private void Cb_OnEnemyHit(MsgOnEnemyHit msg)
    {
        BulletsSynth.Play();
    }
}

using Frictionless;
using UnityEngine;

public class UpgradeSpawner : MonoBehaviour
{
    public GameObject Upgrade;

    MessageRouter _router;

    WeightedRandomBag<EUpgrade> upgradeBag;

    private void Awake()
    {
        upgradeBag = new WeightedRandomBag<EUpgrade>();
        upgradeBag.AddEntry(EUpgrade.BPM, 1f);
        upgradeBag.AddEntry(EUpgrade.AddMelodyImproviserToPlayer, .15f);
        upgradeBag.AddEntry(EUpgrade.AddMarchImproviserToPlayer, .1f);
        upgradeBag.AddEntry(EUpgrade.AddMarchImproviserToEnemy, .05f);
        upgradeBag.AddEntry(EUpgrade.GenerateEnemyBeat, .025f);
        upgradeBag.AddEntry(EUpgrade.GenerateEnemyMelody, .025f);
        upgradeBag.AddEntry(EUpgrade.GeneratePlayerBeat, .05f);
        upgradeBag.AddEntry(EUpgrade.GeneratePlayerMelody, .05f);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        _router = ServiceFactory.Instance.Resolve<MessageRouter>();
        _router.AddHandler<MsgOnUpgradeTaken>(OnUpgradeToken);
        _router.AddHandler<MsgOnEnemyDestroyed>(Cb_OnEnemyDestroyed);
    }

    private void OnDisable()
    {
        _router?.RemoveHandler<MsgOnUpgradeTaken>(OnUpgradeToken);
        _router?.RemoveHandler<MsgOnEnemyDestroyed>(Cb_OnEnemyDestroyed);
    }

    private void OnUpgradeToken(MsgOnUpgradeTaken msg)
    {
        var instruments = InstrumentsManager.Instance;

        switch (msg.Type)
        {
            case EUpgrade.BPM:
                Z.ZMarchPlayer.BPM += (int)msg.Value;
                break;
            case EUpgrade.GenerateEnemyBeat:
                //instruments.EnemyBeatMaker?.GenerateNewBeat();
                break;
            case EUpgrade.GenerateEnemyMelody:
                //instruments.EnemySpawnerDrum?.GenerateNewNotes();
                break;
            case EUpgrade.GeneratePlayerBeat:
                //instruments.PlayerBeatMaker?.GenerateNewBeat();
                break;
            case EUpgrade.GeneratePlayerMelody:
                //instruments.PlayerMelodyGenerator?.GenerateMelody();
                break;
            case EUpgrade.AddMarchImproviserToPlayer:
                instruments.PlayerBeatMaker.Improviser.Add(instruments.GenerateRandomImproviser(instruments.PlayerBeatMaker));
                break;
            case EUpgrade.AddMelodyImproviserToPlayer:
                instruments.PlayerMelodyPlayer.Improviser.Add(instruments.GenerateRandomImproviser(instruments.PlayerMelodyPlayer));
                break;
            case EUpgrade.AddMarchImproviserToEnemy:
                instruments.EnemyBeatMaker.Improviser.Add(instruments.GenerateRandomImproviser(instruments.EnemyBeatMaker));
                break;
        }
    }

    void AddToInt(ref int targetValue, int v, int min, int max)
    {
        targetValue = Mathf.Clamp(targetValue + v, min, max);
    }

    private void Cb_OnEnemyDestroyed(MsgOnEnemyDestroyed msg)
    {
        if (Random.value < 0.75f) return;

        var instance = Instantiate(Upgrade, msg.enemy.transform.position, Quaternion.identity);
        var up = instance.GetComponent<Upgrade>();
        up.SetType(RandomUpgradeType());

        switch (up.Type)
        {
            default:
            case EUpgrade.BPM:
                up.Value = Mathf.RoundToInt((Random.Range(-1.0f, 1.0f)/2f)*10f);
                break;
        }

        instance.GetComponent<EnemyMaterialController>().UpgradeValue = ((int)up.Value) < 0 ? 0f : 1f;
    }

    EUpgrade RandomUpgradeType()
    {
        return upgradeBag.GetRandom();
        /*
        System.Array v = System.Enum.GetValues(typeof(EUpgrade));
        return (EUpgrade)v.GetValue(Random.Range(0, v.Length));
        */
    }

    T RandomEnum<T>()
    {
        System.Array v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(Random.Range(0, v.Length));
    }
}

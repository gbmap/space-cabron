using Frictionless;
using UnityEngine;

public class UpgradeSpawner : MonoBehaviour
{
    public GameObject Upgrade;

    MessageRouter _router;

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
        switch (msg.Type)
        {
            case EUpgrade.BPM:
                BeatMaker.BPM += (int)msg.Value;
                break;
            case EUpgrade.BeatsInBar:
                AddToInt(ref InstrumentsManager.Instance.EnemySpawnerDrum.BeatMaker.BeatsInBar, (int)msg.Value, 4, 8);
                break;
            case EUpgrade.MaxSubBeats:
                AddToInt(ref InstrumentsManager.Instance.EnemySpawnerDrum.BeatMaker.MaxSubBeats, (int)msg.Value, 1, 4);
                break;
            case EUpgrade.NBeats:
                AddToInt(ref InstrumentsManager.Instance.EnemySpawnerDrum.BeatMaker.NBeats, (int)msg.Value, 3, 12);
                break;
            case EUpgrade.AddNoteToPlayerSynth:
                InstrumentsManager.Instance.PlayerSynth.NoteSequencer.AddNote(RandomEnum<ENote>());
                break;
            case EUpgrade.AddNoteToEnemyDrum:
                InstrumentsManager.Instance.EnemySpawnerDrum.AddNote(RandomEnum<EInstrumentAudio>());
                break;
        }
    }

    void AddToInt(ref int targetValue, int v, int min, int max)
    {
        targetValue = Mathf.Clamp(targetValue + v, min, max);
    }

    private void Cb_OnEnemyDestroyed(MsgOnEnemyDestroyed msg)
    {
        if (Random.value < 0.9f) return;

        var instance = Instantiate(Upgrade, msg.enemy.transform.position, Quaternion.identity);
        var up = instance.GetComponent<Upgrade>();
        up.Type = RandomUpgradeType();

        switch (up.Type)
        {
            case EUpgrade.BPM:
                up.Value = Mathf.RoundToInt((Random.Range(-1.0f, 1.0f)/2f)*10f);
                break;
            case EUpgrade.BeatsInBar:
            case EUpgrade.MaxSubBeats:
            case EUpgrade.NBeats:
                up.Value = Random.value < 0.1f ? -1 : 1;
                break;
        }
    }

    EUpgrade RandomUpgradeType()
    {
        System.Array v = System.Enum.GetValues(typeof(EUpgrade));
        return (EUpgrade)v.GetValue(Random.Range(0, v.Length));
    }

    T RandomEnum<T>()
    {
        System.Array v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(Random.Range(0, v.Length));
    }
}

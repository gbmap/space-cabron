using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
using SpaceCabron.Gameplay;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName="Space Cabrón/Level/Instrument Configuration")]
public class InstrumentConfiguration : ScriptableObject, ICloneable<InstrumentConfiguration>
{
    public int BPM = 30;
    public int MaxBPM = 120;
    public ScriptableMelodyFactory MelodyFactory;
    public ImprovisationConfiguration ImprovisationConfiguration;
    public TextAssetPool PossibleStartingInstruments;

    public InstrumentConfiguration Clone()
    {
        var instance = ScriptableObject.CreateInstance<InstrumentConfiguration>();
        instance.BPM = BPM;
        instance.MaxBPM = MaxBPM;
        instance.MelodyFactory = MelodyFactory;
        instance.ImprovisationConfiguration = ImprovisationConfiguration != null ? ImprovisationConfiguration.Clone() : null;
        instance.PossibleStartingInstruments = PossibleStartingInstruments != null ? PossibleStartingInstruments.Clone() as TextAssetPool : null;
        return instance;

        // return new InstrumentConfiguration
        // {
        //     BPM = BPM,
        //     MelodyFactory = MelodyFactory != null ? MelodyFactory.Clone() : null,
        //     ImprovisationConfiguration = ImprovisationConfiguration != null ? ImprovisationConfiguration.Clone() : null,
        //     PossibleStartingInstruments = PossibleStartingInstruments != null ? PossibleStartingInstruments.Clone() as TextAssetPool : null
        // };
    }

    public MelodyFactory GetMelodyFactory(bool useLastUsed) {
        if (useLastUsed && MelodyFactory.LastUsedFactory != null)
            return MelodyFactory.LastUsedFactory;
        else
            return MelodyFactory;
    }

    public void ConfigureTurntable(
        ITurntable turntable, 
        bool useLastUsedFactory
    ) {
        Melody melody = GetMelodyFactory(useLastUsedFactory).GenerateMelody();
        turntable.SetMelody(melody);
        ImprovisationConfiguration.Apply(turntable);
    }

}
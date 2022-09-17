using System.Collections;
using System.Collections.Generic;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
using SpaceCabron.Messages;
using UnityEngine;
using static SpaceCabron.Messages.MsgSpawnDrone;

public class SynthGeneratorButtons : MonoBehaviour
{
    [System.Serializable]
    public class GeneratorConfig {
        public TextAssetPool PatchPool;
        public TurntableBehaviour Turntable;
        public void RandomizeInstrument() {
            Turntable.GetComponentInChildren<HelmProxy>().LoadPatch(PatchPool.GetNext());
        }
    }

    [Header("Player")]
    public TurntableBehaviour PlayerTurntable;

    public ScriptableMelodyFactory MelodyFactory;
    public ImprovisationPool Improvisations;

    public GeneratorConfig Player;
    public GeneratorConfig Metronome;
    public GeneratorConfig Ambient;

    public void RandomizePlayerMelody() {
        IMelodyPlayer.Generate(Player.Turntable.gameObject, MelodyFactory);
    }

    public void LoadRandomSynth() {
        Player.RandomizeInstrument();
    }

    public void LoadMetronomeRandomSynth() {
        Metronome.RandomizeInstrument();
    }

    public void LoadAmbientRandomSynth() {
        Ambient.RandomizeInstrument();
    }

    public void AddPlayerImprovisation() {
        PlayerTurntable.ApplyImprovisation(Improvisations.GetNext().Get(), false);
    }


    public void SpawnDroneMelody() {
        MessageRouter.RaiseMessage(new MsgSpawnDrone{
            DroneType = EDroneType.Melody
        });
    }

    public void DestroyDroneMelody() {
        var drone = GameObject.FindGameObjectWithTag("Drone");
        if (drone != null)
            GameObject.Destroy(drone);
    }
}

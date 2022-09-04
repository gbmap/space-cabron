using System.Linq;
using AudioHelm;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.Gun;
using Gmap.Instruments;
using Gmap.ScriptableReferences;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class DroneSpawner : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public GameObject DronePrefab;
        public GameObject DroneMelodyPrefab;
        public GameObject DroneEveryNPrefab;
        public GameObject PlayerPrefab;
        public FloatReference EnergyValue;

        public UnityEngine.Audio.AudioMixerGroup[] Groups;

        private InstrumentConfiguration DroneInstrument;
        private bool[] hasInjectedPatchArray = new bool[MIXER_GROUP_DRONE_COUNT];
        private const int MIXER_GROUP_DRONE_INDEX = 5;
        private const int MIXER_GROUP_DRONE_COUNT = 3;
        private int numberOfMelodyDronesSpawned = 0;

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgSpawnDrone>(SpawnDrone);
            MessageRouter.AddHandler<MsgSpawnPlayer>(SpawnPlayer);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgSpawnDrone>(SpawnDrone);
            MessageRouter.RemoveHandler<MsgSpawnPlayer>(SpawnPlayer);
        }

        private void SpawnDrone(MsgSpawnDrone msg)
        {
            if (msg.Player == null)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                if (players.Length > 0)
                    msg.Player = players[Random.Range(0, players.Length)];
            }

            Vector3 targetPosition = Vector3.zero;
            if (msg.Player != null)
                targetPosition = msg.Player.transform.position;

            var instance = Instantiate(
                DroneTypeToPrefab(msg.DroneType),
                targetPosition,
                Quaternion.identity
            );

            ConfigureDrone(instance, msg.Player);

            msg.OnSpawned?.Invoke(instance);
            MessageRouter.RaiseMessage(new MsgOnDroneSpawned {
                Drone = instance
            });
        }

        public void ConfigureDrone(GameObject instance, GameObject playerInstance)
        {
            if (playerInstance == null)
                return;

            FollowAtAnOffset offset = instance.GetComponent<FollowAtAnOffset>();
            offset.Offset = UnityEngine.Random.insideUnitSphere;
            offset.Offset.z = 0f;
            offset.Offset.Normalize();
            offset.Target = playerInstance.transform;

            RepeatNoteWithStep step = instance.GetComponent<RepeatNoteWithStep>();
            if (step != null)
            {
                step.UpdateReferences(playerInstance);

                int steps = new int[] { 3, 5, 7, 15, 17, 19 }[UnityEngine.Random.Range(0, 6)];
                step.Steps = steps;

                // GunBehaviour gun = instance.GetComponent<GunBehaviour>();
                // step.OnNoteRepeated += (args) =>
                // {
                //     gun.Fire(new FireRequest
                //     {
                //         BulletScale = 1f,
                //         Special = false
                //     });
                // };
            }

            // Means this is a melody drone.
            TurntableBehaviour turntable = instance.GetComponentInChildren<TurntableBehaviour>();
            if (turntable != null)
                ConfigureMelodyDrone(instance, turntable);
        }

        private GameObject DroneTypeToPrefab(MsgSpawnDrone.EDroneType droneType)
        {
            switch (droneType)
            {
                case MsgSpawnDrone.EDroneType.Random:
                    return DronePrefab;
                case MsgSpawnDrone.EDroneType.Melody:
                    return DroneMelodyPrefab;
                case MsgSpawnDrone.EDroneType.EveryN:
                    return DroneEveryNPrefab;
                default:
                    return DronePrefab;
            }
        }

        private void ConfigureMelodyDrone(GameObject instance, ITurntable turntable)
        {
            int droneAudioMixerIndex = numberOfMelodyDronesSpawned++ % MIXER_GROUP_DRONE_COUNT;

            HelmController controller = instance.GetComponentInChildren<HelmController>();
            controller.channel = MIXER_GROUP_DRONE_INDEX + droneAudioMixerIndex;

            AudioSource audioSource = instance.GetComponentInChildren<AudioSource>();
            audioSource.outputAudioMixerGroup = Groups[droneAudioMixerIndex];

            InjectPatch(instance, droneAudioMixerIndex);
            ConfigureObjectWithNewMelody(instance, turntable);
        }

        private void ConfigureObjectWithNewMelody(GameObject instance, ITurntable turntable)
        {
            if (DroneInstrument == null)
                return;

            DroneInstrument.ConfigureTurntable(turntable, true);
            var tt = instance.GetComponent<InjectTurntableMelodyNotationOnAwake>();
            if (tt != null)
                Destroy(tt);
        }

        private void InjectPatch(GameObject instance, int droneAudioMixerIndex)
        {
            if (hasInjectedPatchArray[droneAudioMixerIndex])
            {
                InjectPatchOnAwake patch = instance.GetComponentInChildren<InjectPatchOnAwake>();
                Destroy(patch);
            }
            else
            {
                // Patch is injected through the InjectPatchOnAwake component.
                hasInjectedPatchArray[droneAudioMixerIndex] = true;
            }
        }

        public void Configure(LevelConfiguration configuration)
        {
            DroneInstrument = configuration.GetInstrumentConfigurationByTag("Player");
        }

        private void SpawnPlayer(MsgSpawnPlayer msg)
        {
            if (EnergyValue)
                EnergyValue.Value = 1f;

            Vector3 position = Vector3.zero;
            if (msg.TargetPosition != null)
                position = msg.TargetPosition.transform.position;
            else
                position = msg.Position;

            GameObject instance = Instantiate(PlayerPrefab, position, Quaternion.identity);
            instance.AddComponent<TemporaryInvincibility>();

            // Configure player turntable with new last melody
            ITurntable turntable = instance.GetComponentInChildren<ITurntable>();
            ConfigureObjectWithNewMelody(instance, turntable);

            // Update drones following the player.
            GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
            drones.Select(d=> d.GetComponent<FollowAtAnOffset>())
                  .ToList().ForEach(f => f.Target = instance.transform);

            System.Array.ForEach(drones, d => {
                RepeatNoteWithStep step = d.GetComponent<RepeatNoteWithStep>();
                if (step != null)
                    step.UpdateReferences(instance);
            });

            msg.OnSpawned?.Invoke(instance);
            MessageRouter.RaiseMessage(new MsgOnPlayerSpawned
            {
                Player = instance
            });
        }

    }
}
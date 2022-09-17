using System.Linq;
using AudioHelm;
using Frictionless;
using Gmap;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
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
            if (playerInstance != null)
            {
                instance.name = instance.name + playerInstance.name[playerInstance.name.Length-1];

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
                }
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

        private void ConfigurePlayerWithNewMelody(GameObject instance, TurntableBehaviour turntable)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 1) {
                var otherPlayer = players.FirstOrDefault(p => p.name[p.name.Length-1] != instance.name[instance.name.Length-1]);
                if (otherPlayer == null)
                {
                    ConfigureObjectWithNewMelody(instance, turntable);
                    return;
                }

                TurntableBehaviour otherTurntable = otherPlayer.GetComponentInChildren<TurntableBehaviour>();
                turntable.ForceSetTurntable(otherTurntable.Turntable as Turntable);

                instance.GetComponent<MelodySwitcher>().Generate(DroneInstrument.GetMelodyFactory(true));


                var update = instance.GetComponent<UpdateBackgroundShaderGlobalVariables>();
                Destroy(update);

                var tt = instance.GetComponentInChildren<InjectTurntableMelodyNotationOnAwake>();
                if (tt != null)
                    Destroy(tt);
            }
            else
            {
                ConfigureObjectWithNewMelody(instance, turntable);
            }
        }

        private void ConfigureObjectWithNewMelody(GameObject instance, ITurntable turntable)
        {
            if (DroneInstrument == null)
            {
                if (LevelLoader.CurrentLevelConfiguration is LevelConfiguration levelConfiguration)
                    Configure(levelConfiguration);

                if (DroneInstrument == null)
                {
                    Debug.LogWarning("[DroneSpawner] Couldn't configure object with melody: no DroneInstrument configured.");
                    return;
                }
            }

            DroneInstrument.ConfigureTurntable(turntable, true);
            IMelodyPlayer.Generate(instance, DroneInstrument.GetMelodyFactory(true));
            var tt = instance.GetComponentInChildren<InjectTurntableMelodyNotationOnAwake>();
            if (tt != null)
                Destroy(tt);

            ConfigureTurntableWithLevelConfiguration.LoadInstrument(instance, DroneInstrument);
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

            Vector3 startingPosition = CalculatePlayerStartingPosition(msg);

            GameObject instance = SpawnPlayer(startingPosition);
            UpdatePlayerName(instance, msg.PlayerIndex);
            MakeInvincible(instance);
            SetupPlayerBrain(instance, msg.PlayerIndex);
            ConfigurePlayerWithNewMelody(instance, instance.GetComponentInChildren<TurntableBehaviour>());
            PlayStartingAnimationIfNotRespawned(instance, msg);
            MakeDronesFollowAndUpdateMelody(instance);

            msg.OnSpawned?.Invoke(instance);
            MessageRouter.RaiseMessage(new MsgOnPlayerSpawned
            {
                Player = instance
            });
        }

        private void UpdatePlayerName(GameObject instance, int playerIndex)
        {
            instance.name = instance.name.Replace("(Clone)", "") + playerIndex.ToString();
        }

        private static Vector3 CalculatePlayerStartingPosition(MsgSpawnPlayer msg)
        {
            Vector3 position = Vector3.zero;
            if (msg.TargetPosition != null)
                position = msg.TargetPosition.transform.position;
            else
                position = msg.Position;
            return position;
        }

        private GameObject SpawnPlayer(Vector3 position)
        {
            return Instantiate(PlayerPrefab, position, Quaternion.identity);
        }

        private static void MakeInvincible(GameObject instance)
        {
            instance.AddComponent<TemporaryInvincibility>();
        }

        private static void SetupPlayerBrain(GameObject instance, int playerIndex)
        {
            var injectBrain = instance.GetComponent<InjectBrainToActor<InputState>>();
            if (injectBrain)
                Destroy(injectBrain);

            ScriptableInputBrain brain = ScriptableObject.CreateInstance<ScriptableInputBrain>();
            brain.Index = playerIndex;
            InjectBrainToActor<InputState>.Inject(instance, brain);
        }

        private void PlayStartingAnimationIfNotRespawned(GameObject instance, MsgSpawnPlayer msg)
        {
            if (!msg.IsRespawn)
            {
                var brain = AnimationBrain.Play<BeginLevelBrain>(instance);
                brain.Index = msg.PlayerIndex;
            }
        }

        private static void MakeDronesFollowAndUpdateMelody(GameObject instance)
        {
            int playerIndex = instance.name[instance.name.Length-1] - '0';
            GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone")
                                            .Where(d=>d.name[d.name.Length-1] == playerIndex.ToString()[0])
                                            .ToArray();
            drones.Select(d => d.GetComponent<FollowAtAnOffset>())
                  .ToList().ForEach(f => f.Target = instance.transform);

            System.Array.ForEach(drones, d =>
            {
                RepeatNoteWithStep step = d.GetComponent<RepeatNoteWithStep>();
                if (step != null)
                    step.UpdateReferences(instance);
            });
        }
    }
}
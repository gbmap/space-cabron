using AudioHelm;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.Gun;
using Gmap.Instruments;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class DroneSpawner : MonoBehaviour
    {
        public GameObject DronePrefab;
        public GameObject DroneMelodyPrefab;
        public GameObject DroneEveryNPrefab;

        public UnityEngine.Audio.AudioMixerGroup[] Groups;

        private bool hasInjectedPatch = false;
        private bool[] hasInjectedPatchArray = new bool[MIXER_GROUP_DRONE_COUNT];
        private const int MIXER_GROUP_DRONE_INDEX = 5;
        private const int MIXER_GROUP_DRONE_COUNT = 3;
        private int numberOfMelodyDronesSpawned = 0;

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgSpawnDrone>(SpawnDrone);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgSpawnDrone>(SpawnDrone);
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

        private void SpawnDrone(MsgSpawnDrone msg)
        {
            var instance = Instantiate(
                DroneTypeToPrefab(msg.DroneType), 
                msg.Player.transform.position, 
                Quaternion.identity
            );

            FollowAtAnOffset offset = instance.GetComponent<FollowAtAnOffset>();
            offset.Offset = UnityEngine.Random.insideUnitSphere;
            offset.Offset.z = 0f;
            offset.Offset.Normalize();
            offset.Target = msg.Player.transform;

            RepeatNoteWithStep step = instance.GetComponent<RepeatNoteWithStep>();
            if (step != null)
            {
                step.Turntable = msg.Player.GetComponentInChildren<TurntableBehaviour>();
                step.Proxy = msg.Player.GetComponentInChildren<HelmProxy>();

                int steps = new int[] { 3, 5, 7, 15, 17, 19 }[UnityEngine.Random.Range(0, 6)];
                step.Steps = steps;

                GunBehaviour gun = instance.GetComponent<GunBehaviour>();
                step.OnNoteRepeated += (args) =>
                {
                    gun.Fire(new FireRequest
                    {
                        BulletScale = 1f,
                        Special = false
                    });
                };
            }

            // Means this is a melody drone.
            TurntableBehaviour turntable = instance.GetComponentInChildren<TurntableBehaviour>();
            if (turntable != null)
            {
                int droneAudioMixerIndex = numberOfMelodyDronesSpawned++ % MIXER_GROUP_DRONE_COUNT; 

                HelmController controller = turntable.GetComponent<HelmController>();
                controller.channel = MIXER_GROUP_DRONE_INDEX + droneAudioMixerIndex;

                AudioSource audioSource = turntable.GetComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = Groups[droneAudioMixerIndex];

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

        }
    }
}
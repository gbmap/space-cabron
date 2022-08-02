using System;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gun;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class DroneSpawner : MonoBehaviour
    {
        public GameObject DronePrefab;

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgSpawnDrone>(SpawnDrone);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgSpawnDrone>(SpawnDrone);
        }

        private void SpawnDrone(MsgSpawnDrone msg)
        {
            var instance = Instantiate(DronePrefab, msg.Player.transform.position, Quaternion.identity);
            FollowAtAnOffset offset = instance.GetComponent<FollowAtAnOffset>();
            offset.Offset = UnityEngine.Random.insideUnitSphere;
            offset.Offset.z = 0f;
            offset.Offset.Normalize();
            offset.Target = msg.Player.transform;

            RepeatNoteWithStep step = instance.GetComponent<RepeatNoteWithStep>();
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

    }
}
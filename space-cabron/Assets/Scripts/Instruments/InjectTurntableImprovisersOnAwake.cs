using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class InjectTurntableImprovisersOnAwake : MonoBehaviour
    {
        public ScriptableImprovisation[] Improvisations;
        void Awake()
        {
            var turntable = GetComponent<ITurntable>();
            System.Array.ForEach(Improvisations, x=> turntable.Improviser.AddImprovisation(x.Get()));
            Destroy(this);
        }
    }
}
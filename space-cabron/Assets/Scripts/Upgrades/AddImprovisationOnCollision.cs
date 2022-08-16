using System.Linq;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap
{
    public class AddImprovisationOnCollision : CollisionHandler<TurntableBehaviour>
    {
        public ImprovisationPool ImprovisationPool;
        private ScriptableImprovisation improvisation;

        private new SpriteRenderer renderer;

        void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            improvisation = GetRandomImprovisation();
            if (improvisation.Icon != null)
                renderer.sprite = improvisation.Icon;
        }

        protected override void HandleCollision(TurntableBehaviour t)
        {
            t.Improviser.AddImprovisation(improvisation.Get());
        }

        private ScriptableImprovisation GetRandomImprovisation()
        {
            return ImprovisationPool.GetNext();
        }
    }
}
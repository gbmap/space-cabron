using System.Linq;
using Gmap;
using Gmap.CosmicMusicUtensil;
using UnityEngine;
using Frictionless;

namespace SpaceCabron.Upgrades
{
    public class AddImprovisationOnCollision : CollisionHandler
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
            t.ApplyImprovisation(this.improvisation.Get(), 5);
        }

        private ScriptableImprovisation GetRandomImprovisation()
        {
            return ImprovisationPool.GetNext();
        }
    }
}
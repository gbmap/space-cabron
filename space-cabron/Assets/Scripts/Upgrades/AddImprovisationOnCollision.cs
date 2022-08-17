using System.Linq;
using Gmap;
using Gmap.CosmicMusicUtensil;
using UnityEngine;
using Frictionless;

namespace SpaceCabron.Upgrades
{
    public class MsgOnImprovisationAdded
    {
        public GameObject Object;
        public ITurntable Turntable;
        public Improvisation Improvisation;
    }

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
            var improvisation = this.improvisation.Get();
            MessageRouter.RaiseMessage(new MsgOnImprovisationAdded { 
                Object = t.gameObject, 
                Turntable = t,
                Improvisation = improvisation 
            });
            t.ApplyImprovisation(improvisation, false);
        }

        private ScriptableImprovisation GetRandomImprovisation()
        {
            return ImprovisationPool.GetNext();
        }
    }
}
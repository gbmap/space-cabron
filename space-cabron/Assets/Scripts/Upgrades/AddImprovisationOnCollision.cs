using System.Linq;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap
{
    public class AddImprovisationOnCollision : CollisionHandler<TurntableBehaviour>
    {
        public ImprovisationPool ImprovisationPool;

        protected override void HandleCollision(TurntableBehaviour t)
        {
            var imp = GetRandomImprovisation(t.Turntable);
            UnityEngine.Debug.Log(imp.ToString());
            t.Improviser.AddImprovisation(imp);
        }

        private Improvisation GetRandomImprovisation(ITurntable turntable)
        {
            return ImprovisationPool.GetNext().Get();
        }

        private SelectionStrategy GetRandomSelectionStrategy(ITurntable turntable)
        {
            int selection = UnityEngine.Random.Range(0, 2);
            switch (selection)
            {
                case 0:
                    return new EveryNStrategy(UnityEngine.Random.Range(2, turntable.Melody.Length/2));
                default:
                case 1:
                    int min = UnityEngine.Random.Range(0, turntable.Melody.Length-1);
                    return new RangeStrategy(
                        min,
                        UnityEngine.Random.Range(min, turntable.Melody.Length)
                    );
            }
        }

        private SelectionStrategy GetRandomBarSelectionStrategy(ITurntable turntable)
        {
            var arr = Enumerable.Range(1, 3).Select(i => i*2).ToArray();
            return new EveryNStrategy(arr[UnityEngine.Random.Range(0, arr.Length)]);
        }
    }
}
using System.Linq;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron
{
    public class AddImprovisationOnCollision : CollisionHandler<TurntableBehaviour>
    {
        public ImprovisationPool ImprovisationPool;

        protected override void HandleCollision(TurntableBehaviour t)
        {
            var imp = GetRandomImprovisation(t.Turntable);
            Debug.Log(imp.ToString());
            t.Improviser.AddImprovisation(imp);
        }

        private Improvisation GetRandomImprovisation(ITurntable turntable)
        {
            return ImprovisationPool.GetNext().Get();
            // SelectionStrategy noteSelection = GetRandomSelectionStrategy(turntable);
            // SelectionStrategy barSelection = GetRandomBarSelectionStrategy(turntable);

            // int improvisation = UnityEngine.Random.Range(0, 2);
            // switch (improvisation)
            // {
            //     case 0:
            //         return new BreakNoteImprovisation(
            //             noteSelection, 
            //             barSelection, 
            //             UnityEngine.Random.Range(2, 3)
            //         );
            //     default:
            //     case 1:
            //         return new TransposeNoteImprovisation(
            //             noteSelection, barSelection, UnityEngine.Random.Range(-12, 12)
            //         );
            // }
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
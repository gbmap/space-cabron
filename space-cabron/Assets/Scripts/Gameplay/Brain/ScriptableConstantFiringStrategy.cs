using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Space Cabrón/Brain/Firing Strategy/Constant")]
    public class ScriptableConstantFiringStrategy : ScriptableFiringStrategy
    {
        public override bool GetFire()
        {
            return true;
        }
    }
}
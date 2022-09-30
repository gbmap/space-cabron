using Gmap.ScriptableReferences;
using SpaceCabron.Gameplay.Level;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Space Cabrón/Level/Level List")]
    public class LevelList : ScriptableObjectList<BaseLevelConfiguration> {}
}
using System;
using SpaceCabron.Gameplay.Level;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class VictoryLevelLoader : ILevelLoader
    {
        public void Load()
        {
            GameObject.Instantiate(Resources.Load("VictoryAnimation"));
        }

        public void Unload()
        {
        }
    }

    [CreateAssetMenu(menuName="Space Cabrón/Level/Victory")]
    public class VictoryLevelConfiguration : BaseLevelConfiguration
    {
        public override ILevelConfiguration Clone()
        {
            return ScriptableObject.CreateInstance<VictoryLevelConfiguration>();
        }

        public override ILevelLoader GetLoader(Action OnFinishedLoading = null)
        {
            return new VictoryLevelLoader();
        }
    }
}
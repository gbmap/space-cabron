using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    [CreateAssetMenu(menuName="Space Cabrón/Interactables/Next Level")]
    public class NextLevelInteractable : Interactable
    {
        public override void Interact(InteractArgs args)
        {
            LevelLoader.Load(LevelLoader.CurrentLevelConfiguration.NextLevel);
        }
    }
}
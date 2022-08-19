using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    [CreateAssetMenu(menuName="Space Cabrón/Interactables/Next Level")]
    public class NextLevelInteractable : Interactable
    {
        [SerializeField] Sprite icon;
        public override Sprite Icon => icon;

        public override bool Interact(InteractArgs args)
        {
            LevelLoader.Load(LevelLoader.CurrentLevelConfiguration.NextLevel);
            return true;
        }
    }
}
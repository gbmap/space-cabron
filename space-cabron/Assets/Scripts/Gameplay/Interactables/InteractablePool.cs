using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    [CreateAssetMenu(menuName="Space Cabrón/Interactables/Interactable Pool")]
    public class InteractablePool : ScriptableReferencePool<Interactable> {}
}

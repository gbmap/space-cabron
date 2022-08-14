using Gmap.ScriptableReferences;

namespace SpaceCabron.Gameplay.Interactables
{
    public abstract class Upgrade : Interactable 
    {
        public IntBusReference Price;
        public override void Interact(InteractArgs args)
        {
        }
    }
}
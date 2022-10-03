using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using UnityEngine;

public class InjectBotBrain : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10)) {
            var brain = gameObject.AddComponent<InjectSpaceBrainToActor>();
            brain.Brain = ScriptableObject.CreateInstance<ScriptableBotBrain>();
            gameObject.GetComponent<Health>().CanTakeDamage = false;
        }
    }
}

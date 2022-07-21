using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron
{
    public abstract class ScriptableBrain : ScriptableObject, IBrain
    {
        public abstract InputState GetInputState();
    }

    [CreateAssetMenu(menuName="Space Cabrón/Brain/Input")]
    public class ScriptableInputBrain : ScriptableBrain
    {
        public override InputState GetInputState()
        {
            return new InputState
            {
                Movement = new Vector2(
                    Input.GetAxis("Horizontal"),
                    Input.GetAxis("Vertical")
                ),
                Shoot = Input.GetButtonDown("Jump")
            };
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Gmap/Brain/Input")]
    public class ScriptableInputBrain : ScriptableBrain<InputState>
    {
        public override InputState GetInputState()
        {
            return new InputState
            {
                Movement = new Vector2(
                    Input.GetAxis("Horizontal"),
                    Input.GetAxis("Vertical")
                ),
                Shoot = Input.GetButtonDown("Jump"),
                Pause = Input.GetKeyDown(KeyCode.Escape)
            };
        }
    }
}
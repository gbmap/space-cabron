using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Gmap/Brain/Input")]
    public class ScriptableInputBrain : ScriptableBrain<InputState>
    {
        public InputActionAsset ActionAsset;

        public override InputState GetInputState(InputStateArgs args)
        {
            PlayerInput input = args.Input;
            if (input == null)
            {
                input = args.Object?.GetComponent<PlayerInput>();
                if (input == null)
                    return new InputState{ Movement = Vector2.zero };
            }

            InputAction movement = input.currentActionMap.FindAction("Movement");
            InputAction shoot = input.currentActionMap.FindAction("Jump");
            InputAction pause = input.currentActionMap.FindAction("Pause");
            Vector2 m = movement.ReadValue<Vector2>();
            return new InputState
            {
                Movement = m,
                Shoot = shoot.WasPressedThisFrame(),
                Pause = pause.WasPressedThisFrame()
            };
        }
    }
}
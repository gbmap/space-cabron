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
            InputAction movement = ActionAsset.actionMaps[0].FindAction("Movement");
            InputAction shoot = ActionAsset.actionMaps[0].FindAction("Jump");
            InputAction pause = ActionAsset.actionMaps[0].FindAction("Pause");
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
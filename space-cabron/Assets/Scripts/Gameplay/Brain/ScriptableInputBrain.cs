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
            return new InputState
            {
                Movement = movement.ReadValue<Vector2>(),
                Shoot = shoot.WasPressedThisFrame(),
                Pause = pause.WasPressedThisFrame()
            };
        }
    }
}
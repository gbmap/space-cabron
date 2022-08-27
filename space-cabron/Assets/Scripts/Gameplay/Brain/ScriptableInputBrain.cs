using System;
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

            EColor color = GetColor(input);

            Vector2 m = movement.ReadValue<Vector2>();
            return new InputState
            {
                Movement = m,
                Shoot = color != EColor.None,
                Pause = pause.WasPressedThisFrame(),
                Color = color
            };
        }

        private EColor GetColor(PlayerInput input)
        {
            List<Tuple<string, EColor>> colors = new List<Tuple<string, EColor>>
            {
                new Tuple<string, EColor>("Pink", EColor.Pink),
                new Tuple<string, EColor>("Blue", EColor.Blue),
                new Tuple<string, EColor>("Green", EColor.Green),
                new Tuple<string, EColor>("Yellow", EColor.Yellow)
            };

            foreach (Tuple<string, EColor> color in colors)
            {
                if (input.currentActionMap.FindAction(color.Item1).WasPressedThisFrame())
                    return color.Item2;
            }

            return EColor.None;
        }
    }
}
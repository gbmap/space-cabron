using System;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Gmap/Brain/Input")]
    public class ScriptableInputBrain : ScriptableBrain<InputState>
    {
        public int Index = 0;

        Rewired.Player rewiredPlayer;
        Rewired.Player RewiredPlayer
        {
            get
            {
                if (!Rewired.ReInput.isReady) {
                    Instantiate(Resources.Load("Rewired Input Manager"));
                }

                if (rewiredPlayer == null)
                {
                    rewiredPlayer = Rewired.ReInput.players.GetPlayer(Index);
                }
                return rewiredPlayer;
            }
        }

        public override InputState GetInputState(InputStateArgs args)
        {
            EColor color = GetColor();
            return new InputState
            {
                Movement = RewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical"),
                Shoot = color != EColor.None,
                Pause = RewiredPlayer.GetButtonDown("Pause"),
                Color = color
            };
        }

        private EColor GetColor()
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
                if (RewiredPlayer.GetButtonDown(color.Item1))
                    return color.Item2;
            }

            return EColor.None;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class SetGameStateOnStart : MonoBehaviour
    {
        public GameState State;
        void Start()
        {
            State.ChangeTo();
        }
    }
}
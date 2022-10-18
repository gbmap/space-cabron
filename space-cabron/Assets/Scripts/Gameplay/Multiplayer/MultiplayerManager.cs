using UnityEngine;
using Frictionless;
using System.Collections.Generic;
using Rewired;
using System.Linq;

namespace SpaceCabron.Gameplay.Multiplayer
{
    public class MsgOnPlayerJoined 
    { 
        public int playerIndex; 
    }

    public class MultiplayerManager : MonoBehaviour
    {
        private static int playerCount = 1;
        public static int PlayerCount 
        { 
            get
            {
                return playerCount;
            }

            set
            {
                playerCount = Mathf.Clamp(value, 1, 4);
            }
        }

        void Awake()
        {
            AssertRewired();
            AssignControllers();
        }

        private static void AssertRewired()
        {
            if (!Rewired.ReInput.isReady)
                Instantiate(Resources.Load<GameObject>("Rewired Input Manager"));
        }

        public static void AssignControllers()
        {
            List<ControllerWithMap> controllers = new List<ControllerWithMap>();
            if (Rewired.ReInput.controllers.Joysticks.Count <= 1)
                controllers.Add(Rewired.ReInput.controllers.Keyboard);
            controllers.AddRange(Rewired.ReInput.controllers.Joysticks);

            for (int i = 0; i < Mathf.Min(PlayerCount, controllers.Count); i++)
            {
                var player = Rewired.ReInput.players.GetPlayer(i);
                player.controllers.ClearAllControllers();
                player.controllers.AddController(controllers[i], true);
            }

            if (PlayerCount == 1 && controllers.Count > 1)
            {
                var player = Rewired.ReInput.players.GetPlayer(0);
                player.controllers.AddController(controllers[1], true);
            }
        }

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgOnPlayerJoined>(OnPlayerJoined);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgOnPlayerJoined>(OnPlayerJoined);
        }

        private void OnPlayerJoined(MsgOnPlayerJoined obj)
        {
        }

        public static GameObject GetPlayerWithIndex(int index) {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
            return objects.FirstOrDefault(o=>o.name.Last()-'0' == index);
        }
    }
}
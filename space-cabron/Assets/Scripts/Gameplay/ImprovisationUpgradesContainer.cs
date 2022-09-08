using System;
using System.Collections.Generic;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Gameplay.Interactables;
using SpaceCabron.Messages;
using UnityEngine;
using static SpaceCabron.Messages.MsgSpawnDrone;

namespace SpaceCabron.Gameplay
{
    ///
    /// <summary> 
    /// Holds permament improvisation upgrades for each player and
    /// populates its improviser when the player is respawned.
    /// </summary>
    /// 
    public class ImprovisationUpgradesContainer : MonoBehaviour
    {
        private Dictionary<int, List<Improvisation>> DictPlayerIdToImprovisations 
            = new Dictionary<int, List<Improvisation>>();

        private Dictionary<EDroneType, List<Improvisation>> DictDroneTypeToImprovisations
            = new Dictionary<EDroneType, List<Improvisation>>();

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgOnPermanentImprovisationUpgrade>(Callback_OnPermanentImprovisationUpgrade);
            MessageRouter.AddHandler<MsgOnPlayerSpawned>(Callback_OnPlayerSpawned);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgOnPermanentImprovisationUpgrade>(Callback_OnPermanentImprovisationUpgrade);
            MessageRouter.RemoveHandler<MsgOnPlayerSpawned>(Callback_OnPlayerSpawned);
        }

        private void Callback_OnPermanentImprovisationUpgrade(
            MsgOnPermanentImprovisationUpgrade msg
        ) {
            AddImprovisationToPlayerIndex(msg.Improvisation, msg.playerIndex);
        }

        private void Callback_OnPlayerSpawned(MsgOnPlayerSpawned msg)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length > 1) {
                return;
            }

            TurntableBehaviour turntable = msg.Player.GetComponentInChildren<TurntableBehaviour>();

            int playerIndex = GetPlayerIndexFromGameObject(msg.Player);
            foreach (Improvisation improvisation in GetImprovisationsForPlayerIndex(playerIndex))
                turntable.ApplyImprovisation(improvisation, -1);
        }

        private IEnumerable<Improvisation> GetImprovisationsForPlayerIndex(int playerIndex)
        {
            if (DictPlayerIdToImprovisations.ContainsKey(playerIndex))
                return DictPlayerIdToImprovisations[playerIndex];
            return new List<Improvisation>();
        }

        private void AddImprovisationToPlayerIndex(Improvisation improvisation, int playerIndex)
        {
            if (!DictPlayerIdToImprovisations.ContainsKey(playerIndex))
                DictPlayerIdToImprovisations.Add(playerIndex, new List<Improvisation>());
            DictPlayerIdToImprovisations[playerIndex].Add(improvisation);
        }

        private int GetPlayerIndexFromGameObject(GameObject player)
        {
            try 
            {
                string name = player.name;
                string digit = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
                return int.Parse(digit);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
                return 0;
            }
        }
    }
}
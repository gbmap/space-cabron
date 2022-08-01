using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Space Cabrón/Brain/Movement Strategy/Follow")]
    public class ScriptableFollowMovementStrategy : ScriptableMovementStrategy2D
    {
        public string Tag = "Player";
        private GameObject player;
        private GameObject Player
        {
            get
            {
                if (player == null)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag(Tag);
                    player = players[Random.Range(0, players.Length)];
                }
                return player;
            }
        }
        public override Vector2 GetDirection(MovementStrategyArgs args)
        {
            Vector2 delta = Player.transform.position - args.Object.transform.position;
            return delta.normalized;
        }
    }
}
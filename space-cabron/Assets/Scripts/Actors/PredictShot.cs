using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class PredictShot : ShowerShot
    {
        GameObject player;
        GameObject Player
        {
            get
            {
                if (player == null)
                    player = GameObject.FindGameObjectWithTag("Player");
                return player;
            }
        }

        protected override float GetAngle()
        {
            if (Player == null)
                return 180f;
            Vector2 deltaToPlayer = (Player.transform.position - transform.position).normalized;
            float a = Vector2.SignedAngle(Vector2.up, deltaToPlayer);
            return a;
        }
    }
}
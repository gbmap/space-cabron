using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class EnableEmissionOnLowHealth : MonoBehaviour
    {
        Health health;
        new ParticleSystem particleSystem;
        void Awake()
        {
            health = GetComponentInParent<Health>();
            if (health != null)
                health.OnTakenDamage += Callback_OnDamage;
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void Callback_OnDamage()
        {
            if (health.CurrentHealth < health.MaxHealth / 2)
            {
                var emission = particleSystem.emission;
                emission.enabled = true;
            }
        }
    }
}


using System;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class PlayerFire : Fire
    {
        EColor currentColor = EColor.Pink;

        protected override void Update()
        {
            base.Update();

            if (LastInputState.Color != EColor.None)
            {
                currentColor = LastInputState.Color;
                if (Mathf.Abs(Time.time - lastShotData.Time) < 0.1f)
                    SetBulletColors(currentColor);
            }
        }

        protected override void FireGun(OnNoteArgs args, bool special)
        {
            base.FireGun(args, special);
            SetBulletColors(currentColor);
        }

        private void SetBulletColors(EColor color)
        {
            foreach (var bullet in lastShotData.BulletInstances) 
            {
                if (bullet == null)
                    continue;

                bullet.GetComponent<ColorBullet>().Color = color;
            }
        }
    }
}
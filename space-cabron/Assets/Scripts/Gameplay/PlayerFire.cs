using System;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class PlayerFire : Fire
    {
        EColor currentColor = EColor.Pink;

        EnemyMaterialController materialController;

        public GameObject GuidedBullet;

        protected override void Awake()
        {
            base.Awake();
            materialController = GetComponent<EnemyMaterialController>();
        }

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
            // if (args.NoteIndex == 0 && GuidedBullet != null) {
            //     FireGuidedBullets();
            // }
        }

        private void FireGuidedBullets()
        {
            for (int i = 0; i < 3; i++) {
                var instance = Instantiate(GuidedBullet, transform.position, Quaternion.identity);
                instance.GetComponent<ColorBullet>().Color 
                    = (EColor)UnityEngine.Random.Range(0,4);
            }
        }

        private void SetBulletColors(EColor color)
        {
            foreach (var bullet in lastShotData.BulletInstances) 
            {
                if (bullet == null)
                    continue;

                bullet.GetComponent<ColorBullet>().Color = color;
            }

            if (materialController != null)
                materialController.Color = color;
        }
    }
}
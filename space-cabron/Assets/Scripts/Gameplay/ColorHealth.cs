using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using UnityEngine;

public class ColorHealth : Health 
{
    public List<EColor> ColorLife = new List<EColor>(20);

    private EColor PreviousColor => ColorLife[Mathf.Clamp(CurrentHealth, 0, MaxHealth-1)];
    private EColor CurrentColor => ColorLife[Mathf.Clamp(CurrentHealth-1, 0, MaxHealth-1)];
    private int colorIndex;

    EnemyMaterialController materialController;

    // This prevents multiple colors from being removed
    // by special shots.
    private bool shouldUpdateColor;

    protected override void Awake()
    {
        base.Awake();
        materialController = GetComponentInChildren<EnemyMaterialController>();
        for (int i = 0; i < MaxHealth; i++)
        {
            ColorLife.Add((EColor)Random.Range(0, 4));
        }
        colorIndex = MaxHealth-1;
        materialController.Color = CurrentColor;
    }

    void Update()
    {
        // if (shouldUpdateColor)
        // {
        //     colorIndex--;
        //     materialController.Color = CurrentColor;
        //     shouldUpdateColor = false;
        // }
    }

    public override bool TakeDamage(Bullet bullet, Collider2D collider)
    {
        ColorBullet cb = bullet as ColorBullet;
        if (cb == null 
        || cb.Color == CurrentColor)
        {
            bool tookDamage = base.TakeDamage(bullet, collider);
            if (tookDamage)
            {
                materialController.Color = CurrentColor;
                // shouldUpdateColor = true;
            }
            return tookDamage;
        }
        return false;
    }
}

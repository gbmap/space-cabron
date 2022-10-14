using System.Collections.Generic;
using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;

public class ColorHealth : Health 
{
    [System.Serializable]
    public class ColorHealthItem
    {
        public EColor Color;
        public int Weight;
    }

    [Header("Life Configuration")]
    public List<ColorHealthItem> LifeConfiguration;

    public List<EColor> ColorLife = new List<EColor>(20);

    private EColor PreviousColor => ColorLife[Mathf.Clamp(CurrentHealth, 0, MaxHealth-1)];
    public EColor CurrentColor => ColorLife[Mathf.Clamp(CurrentHealth-1, 0, MaxHealth-1)];
    private int colorIndex;

    EnemyMaterialController materialController;
    EnemyMaterialController[] materialControllers;

    // This prevents multiple colors from being removed
    // by special shots.
    private bool shouldUpdateColor;

    protected override void Awake()
    {
        base.Awake();
        materialController = GetComponentInChildren<EnemyMaterialController>();

        List<EnemyMaterialController> materialControllers = new List<EnemyMaterialController>();
        var controllers = GetComponentsInChildren<EnemyMaterialController>();
        foreach (var controller in controllers) {
            ColorHealth ch = controller.gameObject.GetComponent<ColorHealth>();
            if (ch != null && ch != this) {
                continue;
            }
            materialControllers.Add(controller);
        }
        this.materialControllers = materialControllers.ToArray();


        // ShuffleBag<EColor> bag = new ShuffleBag<EColor>();
        // foreach (var healthItem in LifeConfiguration)
        //     bag.Add(healthItem.Color, healthItem.Weight);

        // ColorLife.AddRange(bag.Next(MaxHealth));
        PopulateLife(HealthGenerator.GetRandom(), 0);

        int maxHealth = MaxHealth;
        while (maxHealth > 0) {
            int batch = Mathf.Min(maxHealth, 10);
            AddLife(HealthGenerator.GetRandom(), batch);
            maxHealth = Mathf.Max(0, maxHealth - batch);
        }

        colorIndex = MaxHealth-1;
        // materialController.Color = CurrentColor;
        SetMaterialColors(CurrentColor);
    }

    void Start() {
        if (materialController != null) {
            materialController.Color = CurrentColor;
        }
    }

    public void AddLife(HealthGenerator generator, int health) {
        ColorLife.AddRange(generator.Generate(health));
    }

    public void PopulateLife(HealthGenerator generator, int health) {
        ColorLife.Clear();
        AddLife(generator, health);
    }

    public void PopulateLife(HealthGenerator generator) {
        PopulateLife(generator, MaxHealth);
    }

    void Update()
    {

    }

    public override bool TakeDamage(Bullet bullet,
        Collider2D collider,
        MonoBehaviour objectFiring
    ) {
        ColorBullet cb = bullet as ColorBullet;
        if (cb == null 
        || cb.Color == CurrentColor)
        {
            bool tookDamage = base.TakeDamage(bullet, collider, objectFiring);
            if (tookDamage) {
                SetMaterialColors(CurrentColor);
            }
            return tookDamage;
        } else {
            if (objectFiring is PlayerFire)
            {
                // FIRE! FIRE!
                var fire = objectFiring as PlayerFire;
                var input = (fire.Brain as ScriptableInputBrain);
                if (input != null) {
                    MessageRouter.RaiseMessage(new MsgOnWrongBulletHit {
                        PlayerIndex = input.Index,
                    });
                }
            }
        }
        
        return false;
    }

    private void SetMaterialColors(EColor color) {
        foreach (var controller in materialControllers) {
            controller.Color = color;
        }
    }
}

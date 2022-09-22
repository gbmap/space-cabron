using System.Linq;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] SpriteRenderer enemyRenderer;
    [SerializeField] ColorHealth colorHealth;
    SpriteRenderer barRenderer;

    public enum ESide
    {
        Left,
        Right,
        Top,
        Bottom
    }
    public ESide Side = ESide.Right;
    public Vector3 Offset = Vector3.zero;
    public Vector3 Scale = Vector3.one;
    private float[] lastColorValues;

    void Start()
    {
        barRenderer = GetComponent<SpriteRenderer>();

        if (colorHealth == null)
            throw new System.Exception("HealthBar requires a ColorHealth component");
        if (enemyRenderer == null)
            throw new System.Exception("HealthBar requires an enemyRenderer component");

        UpdateTransform();

        colorHealth.OnTakenDamage += Callback_OnDamage;
        UpdateHealthBar();
    }

    void FixedUpdate()
    {
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        Vector3 pos = enemyRenderer.bounds.center + Offset;
        Vector3 scale = Scale;
        if (Side == ESide.Left)
        {
            pos.x -= enemyRenderer.bounds.extents.x + 5f/100f;
            scale.x = 0.05f;
            scale.y = enemyRenderer.bounds.size.y + 0.1f;
        }
        else if (Side == ESide.Right)
        {
            pos.x += enemyRenderer.bounds.extents.x + 5f/100f;
            scale.x = 0.05f;
            scale.y = enemyRenderer.bounds.size.y + 0.1f;
        }
        else if (Side == ESide.Top)
        {
            pos.y += enemyRenderer.bounds.extents.y + 5f/100f;
            scale.y = 0.05f;
            scale.x = enemyRenderer.bounds.size.x + 0.1f;
        }
        else if (Side == ESide.Bottom)
        {
            pos.y -= enemyRenderer.bounds.extents.y + 5f/100f;
            scale.y = 0.05f;
            scale.x = enemyRenderer.bounds.size.x + 0.1f;
        }
        scale = new Vector3(scale.x * Scale.x, scale.y * Scale.y, 1f);

        transform.position = pos;
        transform.localScale = scale;
        transform.rotation = Quaternion.identity;
    }

    private void Callback_OnDamage()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (barRenderer == null)
            return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetInt("_NumberOfColors", Mathf.Min(10, colorHealth.MaxHealth));
        mpb.SetInt("_Axis", Side >= ESide.Top ? 1 : 0);
        if (colorHealth.CurrentHealth % 10 == 0 || lastColorValues == null)
        {
            lastColorValues = colorHealth.ColorLife.Take(colorHealth.CurrentHealth)
                                     .TakeLast(Mathf.Min(10, colorHealth.CurrentHealth))
                                     .Select(c=>(float)c)
                                     .ToArray();
        }

        if (lastColorValues.Length > 0)
            mpb.SetFloatArray("_ColorIndexes", lastColorValues );

        int currentHealth = colorHealth.CurrentHealth % 10 == 0 
                            ? 10 
                            : colorHealth.CurrentHealth%10;
        mpb.SetInt("_CurrentHealth", currentHealth);
        barRenderer.SetPropertyBlock(mpb);
    }
}

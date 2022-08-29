using System;
using System.Collections.Generic;
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
        Right
    }
    public ESide Side = ESide.Right;
    public Vector3 Offset = Vector3.zero;
    public Vector3 Scale = Vector3.one;

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
        transform.localPosition = (Side == ESide.Left
                                ? Vector3.left
                                : Vector3.right)
                                * (enemyRenderer.bounds.extents.x + 5f / 100f)
                                + Offset;
        transform.localScale = new Vector3(
            (10f / 100f) * Scale.x,
            (enemyRenderer.bounds.size.y + 10f / 100f) * Scale.y,
            1f * Scale.z
        );
    }

    private void Callback_OnDamage()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        // barRenderer.enabled = colorHealth.CurrentHealth > 1;

        // List<float> indexes = new List<float>();
        // for (int i = colorHealth.CurrentHealth; i > 0; i--)
        //     indexes.Add((float)colorHealth.ColorLife[i-1]);

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetInt("_NumberOfColors", colorHealth.MaxHealth);
        mpb.SetFloatArray("_ColorIndexes", colorHealth.ColorLife.Select(c=>(float)c).ToArray());
        mpb.SetInt("_CurrentHealth", colorHealth.CurrentHealth);
        barRenderer.SetPropertyBlock(mpb);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] SpriteRenderer enemyRenderer;
    [SerializeField] ColorHealth colorHealth;
    SpriteRenderer barRenderer;

    // Start is called before the first frame update
    void Start()
    {
        barRenderer = GetComponent<SpriteRenderer>();

        if (colorHealth == null)
            throw new System.Exception("HealthBar requires a ColorHealth component");
        if (enemyRenderer == null)
            throw new System.Exception("HealthBar requires an enemyRenderer component");

        transform.localPosition = Vector3.right * (enemyRenderer.bounds.extents.x + 5f/100f);
        transform.localScale = new Vector3(10f/100f, enemyRenderer.bounds.size.y + 10f/100f, 1f);

        colorHealth.OnTakenDamage += Callback_OnDamage;
        UpdateHealthBar();
    }

    private void Callback_OnDamage()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        barRenderer.enabled = colorHealth.CurrentHealth > 1;

        int total = 0;
        List<float> indexes = new List<float>();
        for (int i = colorHealth.CurrentHealth; i > 0 && total < 5; i--, total++)
            indexes.Add((float)colorHealth.ColorLife[i-1]);

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetInt("_NumberOfColors", indexes.Count);
        if (indexes.Count > 0)
            mpb.SetFloatArray("_ColorIndexes", indexes.ToArray());
        barRenderer.SetPropertyBlock(mpb);
    }
}

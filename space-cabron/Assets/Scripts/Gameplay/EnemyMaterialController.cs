using Gmap.Gameplay;
using UnityEngine;

[ExecuteAlways]
[ExecuteInEditMode]
public class EnemyMaterialController : MonoBehaviour
{
    public new SpriteRenderer renderer;
    private Material _mat
    {
        get
        {
            return Application.isPlaying ? renderer.material : renderer.sharedMaterial;
        }
    }

    [Range(0f, 1f)]
    public float DamageFactor = 0f;
    float _lastDamageFactor = 0f;

    [Range(0f, 1f)]
    public float SpawnFactor = 0f;
    float _lastSpawnFactor = 0f;

    [Range(0f, 1f)]
    public float UpgradeValue = 0f;
    float _lastUpgradeValue = 0f;

    Health health;

    private float _lastIsResistent = -999f;
    public bool IsResistant = false;

    int _damageId = Shader.PropertyToID("_Damage");
    int _spawnId = Shader.PropertyToID("_Spawn");
    int _upgradeId = Shader.PropertyToID("_Upgrade");
    int _isResistentId = Shader.PropertyToID("_IsResistant");

    bool isUpgrade;

    void Awake()
    {
        health = GetComponentInChildren<Health>();
        if (health)
        {
            health.OnTakenDamage += () => DamageFactor = 1f;
            health.OnSetResistant += (v) => IsResistant = v;
        }
    }

    private void OnEnable()
    {
        isUpgrade = _mat.shader.name == "Unlit/SH_Upgrade";
    }

    private void Update()
    {
        if (renderer == null) return;
        ShaderFloatCheck(_mat, ref _lastDamageFactor, DamageFactor, _damageId);
        ShaderFloatCheck(_mat, ref _lastSpawnFactor, SpawnFactor, _spawnId);
        ShaderFloatCheck(_mat, ref _lastIsResistent, IsResistant?1f:0f, _isResistentId);
        if (isUpgrade)
        {
            ShaderFloatCheck(_mat, ref _lastUpgradeValue, UpgradeValue, _upgradeId);
        }

        DamageFactor = Mathf.Clamp01(DamageFactor - Time.deltaTime*3f);
    }

    private void ShaderFloatCheck(Material m, ref float lastValue, float currentValue, int id)
    {
        if (lastValue != currentValue)
            m.SetFloat(id, currentValue);
        lastValue = currentValue;
    }
}

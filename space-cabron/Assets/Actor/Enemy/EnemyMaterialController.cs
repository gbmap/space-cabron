using UnityEngine;

[ExecuteAlways]
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

    int _damageId = Shader.PropertyToID("_Damage");
    int _spawnId = Shader.PropertyToID("_Spawn");

    private void Update()
    {
        if (renderer == null) return;
        ShaderFloatCheck(_mat, ref _lastDamageFactor, DamageFactor, _damageId);
        ShaderFloatCheck(_mat, ref _lastSpawnFactor, SpawnFactor, _spawnId);
    }

    private void ShaderFloatCheck(Material m, ref float lastValue, float currentValue, int id)
    {
        if (lastValue != currentValue)
            m.SetFloat(id, currentValue);
        lastValue = currentValue;
    }
}

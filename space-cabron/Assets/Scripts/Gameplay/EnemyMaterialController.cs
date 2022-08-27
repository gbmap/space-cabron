using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EnemyMaterialController : MonoBehaviour, IBrainHolder<InputState>
{
    new Renderer renderer;
    private Material _mat
    {
        get
        {
            return Application.isPlaying ? renderer.material : renderer.sharedMaterial;
        }
    }
    
    IBrain<InputState> brain;
    public IBrain<InputState> Brain 
    { 
        get => brain; 
        set => brain = value; 
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

    public EColor Color = EColor.Pink;
    EColor _lastColor = EColor.Pink;


    public Vector2 Direction = Vector2.zero;
    Vector2 _lastDirection;

    Health health;

    private float _lastIsResistent = -999f;
    public bool IsResistant = false;

    int _damageId = Shader.PropertyToID("_Damage");
    int _spawnId = Shader.PropertyToID("_Spawn");
    int _upgradeId = Shader.PropertyToID("_Upgrade");
    int _isResistentId = Shader.PropertyToID("_IsResistant");
    int _directionId = Shader.PropertyToID("_Direction");
    int _colorIndexId = Shader.PropertyToID("_ColorIndex");

    bool isUpgrade;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
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
        if (renderer == null)
            return;

        UpdateDirection();

        ShaderFloatCheck(_mat, ref _lastDamageFactor, DamageFactor, _damageId);
        ShaderFloatCheck(_mat, ref _lastSpawnFactor, SpawnFactor, _spawnId);
        ShaderFloatCheck(_mat, ref _lastIsResistent, IsResistant ? 1f : 0f, _isResistentId);
        ShaderVectorCheck(_mat, ref _lastDirection, Vector2.Lerp(_lastDirection, Direction, Time.deltaTime*3f), _directionId);
        ShaderEColorCheck(_mat, ref _lastColor, Color, _colorIndexId);
        if (isUpgrade)
            ShaderFloatCheck(_mat, ref _lastUpgradeValue, UpgradeValue, _upgradeId);

        DamageFactor = Mathf.Clamp01(DamageFactor - Time.deltaTime * 3f);
    }

    private void UpdateDirection()
    {
        if (Brain == null)
            return;

        Direction = Brain.GetInputState(new InputStateArgs
        {
            Object = gameObject,
            Caller = this
        }).Movement;
    }

    private void ShaderFloatCheck(Material m, ref float lastValue, float currentValue, int id)
    {
        if (lastValue != currentValue)
            m.SetFloat(id, currentValue);
        lastValue = currentValue;
    }

    private void ShaderVectorCheck(Material m, ref Vector2 lastValue, Vector2 currentValue, int id)
    {
        if (lastValue != currentValue)
            m.SetVector(id, currentValue);
        lastValue = currentValue;
    }

    private void ShaderEColorCheck(Material m, ref EColor lastValue, EColor currentValue, int id)
    {
        if (lastValue != currentValue)
            m.SetFloat(id, (int)currentValue);
        lastValue = currentValue;
    }
}

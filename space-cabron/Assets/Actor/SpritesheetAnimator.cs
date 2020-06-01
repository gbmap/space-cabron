using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteAlways]
public class SpritesheetAnimator : MonoBehaviour
{
    ///////////////////////////
    /// STATIC
    static int _FPS = 24;
    static float FrameDelay { get { return (float)_FPS / 60; } }

    static int _MainTexId = Shader.PropertyToID("_MainTex");
    static int _TilesId = Shader.PropertyToID("_Tiles");
    static int _FrameId = Shader.PropertyToID("_Frame");


    ///////////////////////////
    /// INSPECTOR
    public Vector2Int Cells = new Vector2Int(2, 0);
    public Vector2Int FrameRange = new Vector2Int(0, 1);
    public int CurrentFrame { get; private set; }
    public int CurrentFrameInRange
    {
        get
        {
            return (FrameRange.x + CurrentFrame) % FrameRange.y;
        }
    }


    ///////////////////////////
    /// REFERENCES
    SpriteRenderer _rndr;
    Material _mat;


    ///////////////////////////
    /// PRIVATE
    float _lastFrameChange;

    private void Awake()
    {
        _rndr = GetComponent<SpriteRenderer>();
        _mat = _rndr.sharedMaterial;

        //Vector2 ntiles = Cells;
        _mat.SetVector(_TilesId, new Vector4(Cells.x, Cells.y, 0f, 0f));
        _mat.SetInt(_FrameId, CurrentFrameInRange);
    }

    void UpdateFrameInMaterial(int frame)
    {
        _mat.SetInt(_FrameId, frame);
    }

    Vector2 GetNumberOfTiles(Vector2Int cells, Vector2 spriteSize)
    {
        return spriteSize / cells;
    }

    void Update()
    {
        if (Time.time > _lastFrameChange + FrameDelay)
        {
            CurrentFrame++;
            UpdateFrameInMaterial(CurrentFrameInRange);
            _lastFrameChange = Time.time;
        }
    }
}

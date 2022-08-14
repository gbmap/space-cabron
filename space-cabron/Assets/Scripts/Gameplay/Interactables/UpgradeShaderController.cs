using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    public class UpgradeShaderController : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float UpgradeType;
        new Renderer renderer;

        void Awake()
        {
            renderer = GetComponent<Renderer>();
        }

        void Update()
        {
            renderer.material.SetFloat("_Upgrade", UpgradeType);
        }

        public void SetUpgradeType(float v)
        {
            UpgradeType = v;
        }
    }
}
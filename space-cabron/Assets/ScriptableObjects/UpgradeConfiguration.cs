using UnityEngine;

public class UpgradeConfiguration : ScriptableObject
{
    public Sprite[] sprites;

    public Sprite GetSprite(EUpgrade upgrade)
    {
        return sprites[(int)upgrade];
    }
}

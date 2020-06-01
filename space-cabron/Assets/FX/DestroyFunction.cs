using UnityEngine;
using ObjectPool;

public class DestroyFunction : MonoBehaviour, IObjectPoolEventHandler
{
    public void AnimDestroy()
    {
        this.DestroyOrDisable();
    }

    public void PoolReset()
    {

    }
}

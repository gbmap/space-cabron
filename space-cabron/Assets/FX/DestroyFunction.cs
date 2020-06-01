using UnityEngine;
using ObjectPool;

public class DestroyFunction : MonoBehaviour, IObjectPoolEventHandler
{
    public void Destroy()
    {
        if (GetComponent<ObjectPoolBehavior>())
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {

    }
}

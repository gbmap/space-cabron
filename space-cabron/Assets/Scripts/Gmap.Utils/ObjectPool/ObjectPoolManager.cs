using UnityEngine;
using Useful;

namespace Managers
{
    class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        // BLAAAAAAAAAAAAARGH
        public void DestroyHack(GameObject obj)
        {
            Destroy(obj);
        }
    }
}

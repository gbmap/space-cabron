using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace ObjectPool
{
    public interface IObjectPoolEventHandler : IEventSystemHandler
    {
        void PoolReset();
    }
}

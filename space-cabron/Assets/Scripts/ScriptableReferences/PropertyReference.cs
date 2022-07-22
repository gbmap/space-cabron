using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [System.Serializable]
    public class PropertyReference<ValueType, WrappedType> where WrappedType : ValueReference<ValueType>
    {
        public WrappedType GetFrom;
        public WrappedType SendTo;

        [SerializeField] ValueType _value;
        public ValueType Value
        {
            get
            {
                if (GetFrom)
                    return GetFrom.Value;
                else
                    return _value; 
            }
            set 
            {
                if (SendTo)
                    SendTo.Value = value;
                _value = value;
            }
        }

        public void Update()
        {
            if (GetFrom)
                _value = GetFrom.Value;
            if (SendTo)
                SendTo.Value = _value;
        }
    }

    [System.Serializable]
    public class IntBusReference : PropertyReference<int, IntReference> {}
}
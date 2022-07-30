using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.UI
{
    public class ScaleByFloatReference : MonoBehaviour
    {
        public Gmap.ScriptableReferences.FloatReference ScaleBy;
        void Update()
        {
            transform.localScale = new Vector3(
                ScaleBy.Value, 
                transform.localScale.y, 
                transform.localScale.z
            );
        }
    }
}

using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.QC.Serializers
{
    public class MonoBehaviourSerializer : PolymorphicQcSerializer<MonoBehaviour>
    {
        public override int Priority => 1000;

        public override string SerializeFormatted(MonoBehaviour value, QuantumTheme theme)
        {
            return value.ToString();
        }
    }
}

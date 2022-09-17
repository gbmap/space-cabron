using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.HelmSynthGenerator {
    public abstract class GenericParameter {
        public float Min;
        public float Max;
        public GenericParameter(float min, float max) {
            Min = min;
            Max = max;
        }

        public abstract float GetValue();

        internal Vector2 GetRange()
        {
            return new Vector2(Min, Max);
        }
    }

    public class FloatParameter : GenericParameter {
        public FloatParameter(float min, float max)
            : base(min, max) {}

        public override float GetValue()
        {
            return UnityEngine.Random.Range(Min, Max);
        }
    }

    public class IntParameter : GenericParameter {
        public IntParameter(int min, int max) 
            : base(min,max) {}

        public override float GetValue()
        {
            return (float)UnityEngine.Random.Range(Mathf.RoundToInt(Min), Mathf.RoundToInt(Max));
        }
    }

    public class WaveformParameter : IntParameter {
        public WaveformParameter() : base(0, 10) {}
    }

    public class BoolParameter : IntParameter {
        public BoolParameter() : base(0, 2) {}
    }

    public class ProbBoolParameter : IntParameter {
        float probability;
        public ProbBoolParameter(float probability=0.5f) : base(0, 0) {
            this.probability = probability;
        }

        public override float GetValue(){
            return Random.value <= probability ? 1f : 0f;
        }
    }

    public class OscillatorVolumeParam : ProbBoolParameter{
        public OscillatorVolumeParam(float p, float min=0.5f, float max=1f) : base(p) {
            this.Min = min;
            this.Max = max;
        }
        public override float GetValue()
        {
            return base.GetValue()*UnityEngine.Random.Range(Min, Max);
        }
    }

    public class EnvParameterA : FloatParameter {
        public EnvParameterA() : base(0.01f, 1.0f) {}
    }

    public class EnvParameterD : FloatParameter {
        public EnvParameterD() : base(0.5f, 0.5f) {}
    }

    public class EnvParameterS : FloatParameter {
        public EnvParameterS() : base(0.0f, 1.0f) {}
    }

    public class EnvParameterR : FloatParameter {
        public EnvParameterR() : base(0.1f, 1.5f) {}
    }

    public class ModulationParam : FloatParameter {
        public ModulationParam() : base(-1f, 1f) {}
    }
}
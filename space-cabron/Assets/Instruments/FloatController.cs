using UnityEngine;

public enum EControlledFloatDriver
{
    Envelope,
    LFO1
}

[System.Serializable]
public class ControlledFloat
{
    public double Value = 1f;
    public double CurrentValue;

    public void UpdateValue(double t)
    {
        double x = Invert ? ValueRange.y : ValueRange.x;
        double y = Invert ? ValueRange.x : ValueRange.y;
        CurrentValue = Synth.dLerp(x, y, t);
    }

    public bool Controlled;
    public Vector2 ValueRange = new Vector2();
    public bool Invert = false;
    public EControlledFloatDriver Driver;

    public ControlledFloat()
    {

    }

    public ControlledFloat(float v)
    {
        Value = v;
    }

    public static double operator *(double a, ControlledFloat b)
    {
        return a * (b.Controlled ? b.CurrentValue : b.Value);
    }
}


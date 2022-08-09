using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[System.Serializable]
public class DummyReference
{
    public int reference;
}

public class Bar : ScriptableObject
{
    public DummyReference reference;
    
    public Bar Clone()
    {
        Bar clone = ScriptableObject.CreateInstance<Bar>();
        clone.reference = new DummyReference { reference = this.reference.reference };
        return clone;
    }
}

public class Foo : ScriptableObject
{
    public Bar Reference;
    public DummyReference reference;

    public Foo Clone()
    {
        Foo clone = ScriptableObject.CreateInstance<Foo>();
        clone.Reference = Reference.Clone();
        clone.reference = reference;
        return clone;
    }

    public bool IsEqualTo(Foo obj)
    {
        return obj.GetHashCode() == this.GetHashCode();
    }
}

public class ScriptableObjectTests
{
    [Test]
    public static void ScriptableObjectCloneDoesntChangeOriginal()
    {
        var dummy = ScriptableObject.CreateInstance<Foo>();
        dummy.reference= new DummyReference { reference = 1 };
        dummy.Reference = ScriptableObject.CreateInstance<Bar>();
        dummy.Reference.reference = new DummyReference() { reference = 2 };

        var dummy2 = dummy.Clone();
        Assert.AreNotEqual(dummy, dummy2);

        dummy2.Reference = null;
        Assert.IsNotNull(dummy);
    }
}

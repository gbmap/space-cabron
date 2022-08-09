using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using Gmap.Utils;

public static class SimpleSerializerTests
{
    private static IEnumerable<char> allowedChars = null;
    public static string[] GetStrings()
    {
            // yield return "a";
        if (allowedChars == null)
        {
            allowedChars = Enumerable.Range(' ', '~')
                                                .Select(i=> Convert.ToChar(i))
                                                .Where(c => !char.IsControl(c));
        }


        return Enumerable.Range(0, 100).Select(i => UnityEngine.Random.Range(1, 100))
                                       .Select(i => new string(allowedChars.OrderBy(c => UnityEngine.Random.Range(1, 9999)).Take(i).ToArray()))
                                       .ToArray();
    }

    // A Test behaves as an ordinary method
    [Test]
    public static void SerializeInt([
        Values(1, -1, 2, -2, 9999, -23525, 5, -345),
        Random(-2147483647, 2147483647, 1000)
    ] int value)
    {
        string serialized = SimpleSerializer.Serialize(value);
        int v = (int)SimpleSerializer.Deserialize(serialized);
        Assert.AreEqual(value, v);
    }

    [Test, TestCaseSource(nameof(GetStrings))]
    public static void SerializeString(string value)
    {
        string serialized = SimpleSerializer.Serialize(value);
        string v = (string)SimpleSerializer.Deserialize(serialized);
        byte[] original = System.Text.Encoding.UTF8.GetBytes(value);
        byte[] result = System.Text.Encoding.UTF8.GetBytes(v);
        Assert.AreEqual(original, result);
    }

    [Test]
    public static void SerializeFloat([
        Values(1.0f, -1.0f, 2.0f, -2.0f, 9999.0f, -23525.0f, 5.0f, -345.0f),
        Random(-3.0E+35f, 3.0E+35f, 1000)
    ] float value)
    {
        string serialized = SimpleSerializer.Serialize(value);
        float v = (float)SimpleSerializer.Deserialize(serialized);
        Assert.AreEqual(value, v);
    }

    [Test]
    public static void SerializeBool([Values(true, false)] bool value)
    {
        string serialized = SimpleSerializer.Serialize(value);
        bool v = (bool)SimpleSerializer.Deserialize(serialized);
        Assert.AreEqual(value, v);
    }

    public static Vector2[] GetVector2s()
    {
        return Enumerable.Range(0, 100)
                         .Select(i => new Vector2(UnityEngine.Random.Range(float.MinValue, float.MaxValue), UnityEngine.Random.Range(float.MinValue, float.MaxValue)))
                         .ToArray();
    }

    [Test, TestCaseSource(nameof(GetVector2s))]
    public static void SerializeVector2(Vector2 v)
    {
        string serialized = SimpleSerializer.Serialize(v);
        Assert.IsTrue(Vector2.Distance(v, (Vector2)SimpleSerializer.Deserialize(serialized)) < 0.0001f);
    }


    public static Vector3[] GetVector3s()
    {
        return Enumerable.Range(0, 100)
                         .Select(i => new Vector3(UnityEngine.Random.Range(float.MinValue, float.MaxValue), UnityEngine.Random.Range(float.MinValue, float.MaxValue), UnityEngine.Random.Range(float.MinValue, float.MaxValue)))
                         .ToArray();
    }    
    
    [Test, TestCaseSource(nameof(GetVector3s))]
    public static void SerializeVector3(Vector3 v)
    {
        string serialized = SimpleSerializer.Serialize(v);
        Assert.IsTrue(Vector2.Distance(v, (Vector3)SimpleSerializer.Deserialize(serialized)) < 0.0001f);
    }
}

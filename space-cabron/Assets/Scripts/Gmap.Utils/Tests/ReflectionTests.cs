using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ReflectionTests
{
    public class TestClass
    {
        public Vector3 position;
        public GameObject gameObject;
        public int value;
    }

    [Test]
    public static void GetField()
    {
        var field = typeof(TestClass).GetField("position", BindingFlags.Public | BindingFlags.Instance);
        Debug.Log(field);
        Assert.IsNotNull(field);
    }

    [Test]
    public static void SetField()
    {
        var field = typeof(TestClass).GetField("position", BindingFlags.Public | BindingFlags.Instance);
        var obj = Activator.CreateInstance(typeof(TestClass));
        field.SetValue(obj, new Vector3(1, 2, 3));
        Assert.AreEqual(new Vector3(1, 2, 3), (Vector3)field.GetValue(obj));
    }

    [Test]
    public static void IsFieldNumeric()
    {
        var field = typeof(TestClass).GetField("value", BindingFlags.Public | BindingFlags.Instance);
        Assert.IsTrue(field.FieldType.IsPrimitive);

        var field2 = typeof(TestClass).GetField("position", BindingFlags.Public | BindingFlags.Instance);
        Assert.IsFalse(field2.FieldType.IsPrimitive);
    }

    [Test]
    public static void GetListOfFields()
    {
        var fields = typeof(TestClass).GetFields(BindingFlags.Public | BindingFlags.Instance);
        Assert.AreEqual(3, fields.Length);
    }

    [Test]
    public static void GetFieldTypes()
    {
        var fields = typeof(TestClass).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
            Debug.Log(field.FieldType);

        string[] expectedTypes = new string[] {
            "vector3",
            "gameobject",
            "int"
        };

        for (int i = 0; i < fields.Length; i++)
        {
            // Order of declaration is not garanteed.
            var field = fields[i];
            string name = field.FieldType.Name.ToLower();
            Assert.IsTrue(expectedTypes.Any(t => name.Contains(t)));
        }
    }

    [Test]
    public static void ObjectPrimitiveType()
    {
        object a = 1;
        Assert.AreEqual(a.GetType(), typeof(int));
    }

}
